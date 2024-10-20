using Microsoft.AspNetCore.SignalR;
using Mythologist_Client_WASM.Client.Infos;
using Mythologist_Client_WASM.Services;
using SharedLogic.Events;
using SharedLogic.Model;
using SharedLogic.Services;

namespace Mythologist_Client_WASM.Hubs
{
	public class GameHub : Hub
	{
		IGameRoomService rooms;
		IDatabaseConnectionService database;
		HttpClient httpClient;

        public GameHub(IGameRoomService _rooms, IDatabaseConnectionService _database, HttpClient _httpClient)
		{
			rooms = _rooms;
			database = _database;
			httpClient = _httpClient;
        }

		public override async Task OnConnectedAsync()
		{
			Console.WriteLine($"Player with ID `{Context.ConnectionId}` connected");

			await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            Console.WriteLine($"Player with ID `{Context.ConnectionId}` disconnected");

			//This group may be null if the game ended up being deleted, or the client wasn't in any games
			string? groupToNotify = rooms.ClientDisconnection(Context.ConnectionId);
			if (groupToNotify is not null)
			{
                Clients.Group(groupToNotify).SendAsync("NotifyOfClients", rooms.GetRoom(groupToNotify).GetClientsInGameAsList());
            }

            return base.OnDisconnectedAsync(exception);
        }

        // See if we're allowed to join the game.
        // If there's a GM password, also validate that.
        public async Task<SuccessOrFailInfo> JoinGame(string gameName, string username, string? discordClientID, Uri? avatarUrl, string? GMPassword)
		{
			bool allowedToJoin = true;
			bool isGM = false;
			string failureMessage = "Error Joining Game";
			if (GMPassword != null)
			{
				allowedToJoin = await VerifyPassword(gameName, GMPassword);
				if (!allowedToJoin)
				{
					failureMessage = "Invalid Password";
				}
				isGM = true;

            }

			//Check that the game exists
			if (!await database.VerifyGameExists(gameName)){
				allowedToJoin = false;
				failureMessage = $"Game '{gameName}' does not exist";
			}

			//Check that the player name is valid
			try {
				var room = rooms.GetRoom(gameName);
				if (room.GetClientsInGameAsList().Any(x => x.userName.Equals(username, StringComparison.OrdinalIgnoreCase))) {
					allowedToJoin = false;
					failureMessage = $"Game '{gameName}' already has a player named '{username}'.";
				}
			}
			catch(Exception ex) {
				//Fine, just throw from getRoom as it's possible it dosen't exist yet if this is the first join.
			}

			if (allowedToJoin)
			{
				//Register the new client
				var groupName = gameName;
				await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
				await rooms.NewClientConnection(groupName, Context.ConnectionId, username, discordClientID, avatarUrl, isGM, database);
                var allScenes = await database.AllScenes(gameName);


				// Check whitelist compatibility
				var playerProperties = await database.PlayerProperties(gameName);
				if (playerProperties.treatAsWhitelist) {
                    if (!playerProperties.playerProperties.Any(x => x.playerName != null ? x.playerName.Equals(username, StringComparison.OrdinalIgnoreCase) : false))
					{
						SuccessOrFailInfo whitelistRejectedFail = new SuccessOrFailInfo();
						whitelistRejectedFail.successful = false;
						whitelistRejectedFail.message = $"'{username}' is not on the whitelist for game '{gameName}'.";
						return whitelistRejectedFail;
					} 
				}

				//If something's gone wrong, like there are no scenes, or the default scene is invalid, this will stop clients entering.
                try
                {
                    //Seriously if the client isn't in a scene it wont display in the frontend .
                    EnsureClientIsInScene(allScenes, rooms.GetRoom(gameName).GetClient(Context.ConnectionId), await database.GameSettings(gameName));
                }
                catch (Exception ex)
                {
                    SuccessOrFailInfo couldntFindSceneFail = new SuccessOrFailInfo();
                    couldntFindSceneFail.successful = false;
					couldntFindSceneFail.message = ex.Message;
					return couldntFindSceneFail;
                }

                Console.WriteLine($"Player with ID `{Context.ConnectionId}` Joined Game '{groupName}'");
                await Clients.Group(gameName).SendAsync("NotifyOfClients", rooms.GetRoom(groupName).GetClientsInGameAsList());
			}

			SuccessOrFailInfo successOrFail = new SuccessOrFailInfo();
            successOrFail.successful = allowedToJoin;
            successOrFail.message = failureMessage;

            return successOrFail;
		}

		private void EnsureClientIsInScene(List<SceneModel> allScenes, ClientInfo client, GameSettingsModel gameSettings)
		{
            if (allScenes.Count == 0)
            {
				throw new Exception("Attempting to load a client into a game with no scenes");
            }
            else
            {
				if ((gameSettings.defaultScene != null) && (allScenes.Any(x => x.id == gameSettings.defaultScene.id)))
				{
					client.currentSceneID = gameSettings.defaultScene.id;
                }
				else
				{
					client.currentSceneID = allScenes.First().id;
				}
            }
        }

		//Called initially, and whenever a client feels like it needs to get all the messages to setup its own state again
		public async Task RefreshGameState(string gameName)
		{
			ClientInfo thisClient = rooms.GetRoom(gameName).GetClient(Context.ConnectionId);

			// Tempted to parralelize this but fuck knows how the caching would work if this is the first entry.
            var allScenes = await database.AllScenes(gameName);
			List<CharacterModel> allCharacters = await database.AllCharacters(gameName);
			var gameSettings = await database.GameSettings(gameName);

			try
			{
				EnsureClientIsInScene(allScenes, thisClient, gameSettings);
			}
			catch(Exception ex)
			{
				await Clients.Client(Context.ConnectionId).SendAsync("NotifyOfServerError", ex.Message);
				return;
			}
            GameInfo gameInfo = new GameInfo { scenes = allScenes, gameSettings = gameSettings, characters = allCharacters };

			FullGameStateInfo fullState  = new FullGameStateInfo(){
				allClients = rooms.GetRoom(gameName).GetClientsInGameAsList(),
				liveCharactersInScenesState = rooms.GetRoom(gameName).GetCharacters().GetAllCharactersAsDict(),
				gameInfo = gameInfo,
				liveGameSettings = rooms.GetRoom(gameName).liveGameSettings
			};

			await Clients.Client(Context.ConnectionId).SendAsync("NotifyOfFullGameState", fullState);
        }

		public async Task ChangeGameSettings(string gameName, GameSettingsInfo settingsInfo)
		{
			rooms.UpdateSettings(gameName, settingsInfo);
            await Clients.Group(gameName).SendAsync("NotifyOfGameSettingsInfo", settingsInfo);

        }

		private async Task<bool> VerifyPassword(string gameName, string GMPassword)
		{
			return await database.VerifyLogin(gameName, GMPassword);
        }

		public async Task ChangeClientScene(string gameName, string clientToChangeSignalRConnectionID, string newScene)
		{
            rooms.GetRoom(gameName).GetClient(clientToChangeSignalRConnectionID).currentSceneID = newScene;
            await Clients.Group(gameName).SendAsync("NotifyOfClients", rooms.GetRoom(gameName).GetClientsInGameAsList());
		}

		public async Task UpdateCharacterState(string gameName, CharacterInfo characterUpdate) {
			var characters = rooms.GetRoom(gameName).GetCharacters();
			characters.UpdateCharacter(characterUpdate.scene, characterUpdate);

			await Clients.Group(gameName).SendAsync("NotifyOfCharactersInScene",  characterUpdate.scene, characters.GetCharacterInfosInSceneAsDict(characterUpdate.scene));
		}
		
		public async Task SendEvent(string gameName, Event theEvent)
		{
			var clientsInGame = rooms.GetRoom(gameName).GetClientsInGameAsDict();

			if (theEvent.TargetUsernames == null)
			{
				Console.WriteLine("ERROR. No Targets in Event");
				return;
			}

			

            List<Task> sendEventMessages = new List<Task>();
            foreach (var connectionID in GetConnectionIdsFromUsernames(theEvent.TargetUsernames, clientsInGame))
			{
				ClientInfo? target;
				if (clientsInGame.TryGetValue(connectionID, out target))
				{
                    sendEventMessages.Add(Clients.Client(connectionID).SendAsync("NotifyOfEvent", theEvent));
				}
			}

			await Task.WhenAll(sendEventMessages);
		}

		private IEnumerable<string> GetConnectionIdsFromUsernames(List<string> usernames, Dictionary<string, ClientInfo> clientsInGame) {
			return clientsInGame.Values.Where(x => usernames.Contains(x.userName)).Select(x => x.signalRConnectionID);
		}
    }
}
