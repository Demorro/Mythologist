using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using SharedLogic.Model;

namespace SharedLogic.Services
{
    public class DatabaseConnectionService : IDatabaseConnectionService
    {
        private CosmosClient cosmosClient;
        private Microsoft.Azure.Cosmos.Database? database = null;
        private IConfiguration configuration;

        private Microsoft.Azure.Cosmos.Container gamesContainer = null;
        private GameModel? gameModel = null;

        private List<SceneModel> _allScenes = new List<SceneModel>();
        public List<SceneModel> AllScenes() { return _allScenes; }

        public SceneModel? Scene(string sceneID) {
            SceneModel? scene = AllScenes().Find(x => x.id == sceneID);
            return scene;
        }

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
            // Hey look an exception for control-flow. How am I meant to do this nicely CosmosAPI? ANSWER ME!?
            bool itemAlreadyExists = false;
            GameModel gameData = null;
            try
            {
                gameData = await gameDataContainer.ReadItemAsync<GameModel>(gameName, partitionKey);
                gameModel = gameData;
                itemAlreadyExists = true;
            }
            catch (Exception)
            {
                itemAlreadyExists = false;
            }

            return itemAlreadyExists ? gameModel : null;
        }

        public async Task HydrateForGame(string gameName)
        {
            var db = await GetDatabaseLazy();
            var container = await GetGameDataContainer(db);
            var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), container);
            _allScenes = model.scenes;

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

        public async Task AddScene(string gameName, SceneModel newScene)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

            if (model == null)
            {
                throw new Exception("Expected existing game model");
            }

            //Consider partial document update to keep costs down
            model.scenes.Add(newScene);
            _allScenes = model.scenes;
            await gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RenameScene(string gameName, SceneModel oldScene, string newSceneName)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

            if (model == null)
            {
                throw new Exception("Expected existing game model");
            }

            int index = model.scenes.FindIndex(x => x.id == oldScene.id);
            if (index == -1)
            {
                throw new Exception($"Could not find scene to rename {oldScene.id}");
            }
            oldScene.id = newSceneName;
            model.scenes[index] = oldScene;

            //Consider partial document update to keep costs down
            _allScenes = model.scenes;
            await gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task UpdateScene(string gameName, SceneModel updatedScene)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

            if (model == null)
            {
                throw new Exception("Expected existing game model");
            }
            
            int index = model.scenes.FindIndex(x => x.id == updatedScene.id);
            if (index == -1)
            {
                throw new Exception($"Could not find scene to update {updatedScene.id}");
            }
            model.scenes[index] = updatedScene;

            //Consider partial document update to keep costs down
            _allScenes = model.scenes;
            await gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task RemoveScene(string gameName, SceneModel sceneToDelete)
        {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
            var gameDataContainer = await GetGameDataContainer(database);
            var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

            if (model == null)
            {
                throw new Exception("Expected existing game model");
            }

            //Consider partial document update to keep costs down
            model.scenes.RemoveAll(x => x.id.Equals(sceneToDelete.id));
            _allScenes = model.scenes;
            await gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
        }

        public async Task UpdateGameSettings(string gameName, GameSettingsModel settings)
        {
			Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
			var gameDataContainer = await GetGameDataContainer(database);
			var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);

			if (model == null)
			{
				throw new Exception("Expected existing game model");
			}

            //Consider partial document update to keep costs down
            model.gameSettings = settings;
			await gameDataContainer.ReplaceItemAsync<GameModel>(model, gameName);
		}

        public async Task<GameSettingsModel> GameSettings(string gameName)
        {
			Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
			var gameDataContainer = await GetGameDataContainer(database);
			var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);
			return model?.gameSettings;

		}

        public async Task<Guid> StorageGuid(string gameName) {
            Microsoft.Azure.Cosmos.Database database = await GetDatabaseLazy();
			var gameDataContainer = await GetGameDataContainer(database);
			var model = await GameDataModelFromDB(gameName, new PartitionKey(gameName), gameDataContainer);
			return model.storageID;
        }
	}
}
