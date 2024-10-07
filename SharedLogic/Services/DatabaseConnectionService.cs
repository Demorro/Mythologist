using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SharedLogic.Model;

namespace SharedLogic.Services
{
    public class DatabaseConnectionService : IDatabaseConnectionService
    {
        private CosmosClient cosmosClient;
        private IConfiguration configuration;

        //Lazily populated in HydrateForGame. The conceit being that each session only goes into one game.
        private Microsoft.Azure.Cosmos.Database? database = null;
        private Microsoft.Azure.Cosmos.Container? gamesContainer = null;

        public DatabaseConnectionService(string cosmosConnectionString)
        {
            initConnection(cosmosConnectionString); ;
        }

        public DatabaseConnectionService(IConfiguration configuration)
        {
            this.configuration = configuration;
            string? cosmosConnectionString = configuration.GetConnectionString("mythologist_db");
            initConnection(cosmosConnectionString);

        }

        private void initConnection(string cosmosConnectionString)
        {
            this.cosmosClient = new CosmosClient(cosmosConnectionString, new CosmosClientOptions() { ApplicationName = "Mythologist_CRUD" });
            if (this.cosmosClient == null)
            {
                throw new Exception($"Failed to connect to CosmosDB endpoint");
            }

            GetDatabaseLazy();
        }


        //You cant just get a database in cosmos. The only way to ensure existence is to do the create if not exists
        //Since constructors can't be async, use a lazy approach like this. I wish I could have a super-private member so you had to go through this.
        //Could do a static I guess but I don't know C# enough to try.

        private async Task<Microsoft.Azure.Cosmos.Database> GetDatabaseLazy()
        {
            string? databaseId = configuration["DatabaseId"];
            if (this.database == null)
            {
                this.database = await this.cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            }

            return this.database;
        }

        //The container that stores the scenes for a particular game. It's just named after the game name
        private async Task<Microsoft.Azure.Cosmos.Container> GetGameDataContainer(Microsoft.Azure.Cosmos.Database database)
        {
            if (gamesContainer != null)
            {
                return gamesContainer;
            }

            string? containerId = configuration["GameDataContainerID"];

            ContainerProperties containerProperties = new ContainerProperties()
            {
                Id = containerId,
                PartitionKeyPath = "/id"
            };
            ThroughputProperties throughputProperties = ThroughputProperties.CreateManualThroughput(400); //400 is min.
            var response = await database.CreateContainerIfNotExistsAsync(containerProperties, throughputProperties);
            switch (response.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                case System.Net.HttpStatusCode.OK: //Ok if it already exists generally
                    break;
                default:
                    throw new Exception($"Unexpected response attempting to get or create new game: {response.StatusCode}");
            }

            gamesContainer = response;
            return gamesContainer;
        }

        private async Task<GameModel?> GameDataModelFromDB(string gameName, Microsoft.Azure.Cosmos.PartitionKey partitionKey, Microsoft.Azure.Cosmos.Container gameDataContainer)
        {
            try
            {
                return await gameDataContainer.ReadItemAsync<GameModel>(gameName, partitionKey);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task CreateNewGame(string gameName, string GMPassword)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();

            var gameInfo = new GameModel(gameName, GMPassword);
            gameInfo.scenes.Add(new SceneModel("Default Scene")); //Games should have at least one scene
            var partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(gameName);

            var gameDataContainer = await GetGameDataContainer(database);
            var createGameResponse = await gameDataContainer.CreateItemAsync(gameInfo, partitionKey);
            switch (createGameResponse.StatusCode)
            {
                case System.Net.HttpStatusCode.Created:
                    break;
                default:
                    throw new Exception($"Unexpected response attempting to create game info: {createGameResponse.StatusCode}");
            }

        }

        public async Task<bool> VerifyGameExists(string gameName)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(gameName);
            GameModel? gameData = await GameDataModelFromDB(gameName, partitionKey, gameDataContainer);

            return gameData != null;
        }

        public async Task<bool> VerifyLogin(string gameName, string GMPassword)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var partitionKey = new Microsoft.Azure.Cosmos.PartitionKey(gameName);
            GameModel? gameData = await GameDataModelFromDB(gameName, partitionKey, gameDataContainer);

            if (gameData == null)
            {
                throw new Exception($"Game name \"{gameName}\" cannot be found");
            }

            if (GMPassword != gameData.GMPassword)
            {
                return false;
            }

            return true;
        }

        class ConnectionObjects {
            public ConnectionObjects(Microsoft.Azure.Cosmos.Database? database, Microsoft.Azure.Cosmos.Container? gameDataContainer, GameModel? gameDataModel) {
                if (database == null) {
                    throw new Exception("Unexpected null database");
                }
                if (gameDataContainer == null) {
                    throw new Exception("Unexpected null game data container");
                }
                if (gameDataModel == null) {
                    throw new Exception("Game does not exist.");
                }

                this.database = database;
                this.gameDataContainer = gameDataContainer;
                this.gameDataModel = gameDataModel;
            }

            public Microsoft.Azure.Cosmos.Database database {get; private set;}
            public Microsoft.Azure.Cosmos.Container gameDataContainer {get; private set;}
            public GameModel gameDataModel {get; private set;}
        }

        private async Task<ConnectionObjects> GameDBConnection(string gameName) {
            Microsoft.Azure.Cosmos.Database? database = await GetDatabaseLazy();
            Microsoft.Azure.Cosmos.Container? gameDataContainer = await GetGameDataContainer(database);
            GameModel? model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

            return new ConnectionObjects(database, gameDataContainer, model);
        }

        public async Task<List<SceneModel>> AllScenes(string gameName) {
            ConnectionObjects connection = await GameDBConnection(gameName);
            return connection.gameDataModel.scenes; 
        }

        public async Task<SceneModel?> Scene(string gameName, string sceneID) {
            SceneModel? scene = (await AllScenes(gameName)).Find(x => x.id == sceneID);
            return scene;
        }

        public async Task AddScene(string gameName, SceneModel newScene)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.scenes.Add(newScene);
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RenameScene(string gameName, SceneModel oldScene, string newSceneName)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            int index = model.scenes.FindIndex(x => x.id == oldScene.id);
            if (index == -1)
            {
                throw new Exception($"Could not find scene to rename {oldScene.id}");
            }
            oldScene.id = newSceneName;
            model.scenes[index] = oldScene;

            //Consider partial document update to keep costs down
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task UpdateScene(string gameName, SceneModel updatedScene)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;
            
            int index = model.scenes.FindIndex(x => x.id == updatedScene.id);
            if (index == -1)
            {
                throw new Exception($"Could not find scene to update {updatedScene.id}");
            }
            model.scenes[index] = updatedScene;

            //Consider partial document update to keep costs down
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RemoveScene(string gameName, SceneModel sceneToDelete)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.scenes.RemoveAll(x => x.id.Equals(sceneToDelete.id));
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task<List<CharacterModel>> AllCharacters(string gameName) {
            ConnectionObjects connection = await GameDBConnection(gameName);
            return connection.gameDataModel.characters; 
        }

        public async Task<CharacterModel?> Character(string gameName, string characterID) {
            CharacterModel? character = (await AllCharacters(gameName)).Find(x => x.id == characterID);
            return character;
        }

        public async Task AddCharacter(string gameName, CharacterModel newCharacter)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.characters.Add(newCharacter);
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RenameCharacter(string gameName, CharacterModel oldCharacter, string newCharacterName)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            int index = model.characters.FindIndex(x => x.id == oldCharacter.id);
            if (index == -1)
            {
                throw new Exception($"Could not find character to rename {oldCharacter.id}");
            }
            oldCharacter.id = newCharacterName;
            model.characters[index] = oldCharacter;

            //Consider partial document update to keep costs down
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task UpdateCharacter(string gameName, CharacterModel updatedCharacter)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;
            
            int index = model.characters.FindIndex(x => x.id == updatedCharacter.id);
            if (index == -1)
            {
                throw new Exception($"Could not find character to update {updatedCharacter.id}");
            }
            model.characters[index] = updatedCharacter;

            //Consider partial document update to keep costs down
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RemoveCharacter(string gameName, CharacterModel characterToDelete)
        {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.characters.RemoveAll(x => x.id.Equals(characterToDelete.id));
            await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task UpdateGameSettings(string gameName, GameSettingsModel settings)
        {
			ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.gameSettings = settings;
			await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
		}

        public async Task<GameSettingsModel> GameSettings(string gameName)
        {
			ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;
			return model.gameSettings;
		}

        public async Task UpdatePlayerProperties(string gameName, PlayerPropertiesModel playerProperties) {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;

            //Consider partial document update to keep costs down
            model.playerProperties = playerProperties;
			await connection.gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task<PlayerPropertiesModel> PlayerProperties(string gameName) {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;
			return model.playerProperties;
        }
        public async Task<Guid> StorageGuid(string gameName) {
            ConnectionObjects connection = await GameDBConnection(gameName);
            var model = connection.gameDataModel;
			return model.storageID;
        }
	}
}
