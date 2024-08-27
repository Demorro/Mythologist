
using DotNetTools.SharpGrabber;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Mythologist_Client_WASM.Client.Infos;
using SharedLogic.Model;
using SharedLogic.Services;
using System.Runtime.CompilerServices;
using static Mythologist_Client_WASM.Client.Services.ISignalRHubClientService;

namespace Mythologist_Client_WASM.Client.Services
{
    public class SignalRHubClientService : ISignalRHubClientService
    {
        private HubConnection? gameHubConnection;
        private NavigationManager navManager;

        private const string GAME_ENDPOINT = "/gamehub";
        private const string AUDIO_STREAM_ENDPOINT = "/audiostreamhub";

        private string? gameName = null;

        public SignalRHubClientService(NavigationManager _navManager)
        {
            this.navManager = _navManager;
        }

        public async Task StopAnyConnections() {
            if(gameHubConnection != null) {
                await gameHubConnection.StopAsync();
                await gameHubConnection.DisposeAsync();
                gameHubConnection = null;
            }

            InjectNotifyEventInfoDelegate(null);
            InjectNotifyOfGameInfoDelegate(null);
            InjectNotifyOfClientsDelegate(null);
            InjectNotifyOfCharactersInSceneDelegate(null);
            InjectNotifyOfGameSettingsInfoDelegate(null);
            InjectNotifyEventInfoDelegate(null);
            InjectNotifyOfServerErrorDelegate(null);
        }

        /*
         * Call early. Probably on landing
         */
        public async Task InitializeConnectionAndJoinGame(string gameName, string userName, Utils.DiscordUser? discordUserObject, string? GMPassword)
        {
            if(gameHubConnection != null) {
                await gameHubConnection.DisposeAsync();
            }

            gameHubConnection = new HubConnectionBuilder().WithUrl(navManager.ToAbsoluteUri(GAME_ENDPOINT)).Build();

            gameHubConnection.On<FullGameStateInfo>("NotifyOfFullGameState", NotifyOfFullGameState);
            gameHubConnection.On<GameInfo>("NotifyOfGameInfo", NotifyOfGameInfo);
            gameHubConnection.On<List<ClientInfo>>("NotifyOfClients", NotifyOfClients);
            gameHubConnection.On<(string, Dictionary<string, CharacterInfo>)>("NotifyOfCharactersInScene", NotifyOfCharactersInScene); //(sceneId, <characterID, Character>)
            gameHubConnection.On<GameSettingsInfo>("NotifyOfGameSettingsInfo", NotifyOfGameSettingsInfo);
            gameHubConnection.On<EventInfo>("NotifyOfEventInfo", NotifyOfEventInfo);
            gameHubConnection.On<string>("NotifyOfServerError", NotifyOfServerError);

            await gameHubConnection.StartAsync();
            this.gameName = gameName;
           
            SuccessOrFailInfo joinSuccess = await gameHubConnection.InvokeAsync<SuccessOrFailInfo>("JoinGame", gameName, userName, discordUserObject?.Id, discordUserObject?.AvatarUrl(), GMPassword);

            if (!joinSuccess.successful)
            {
                await gameHubConnection.DisposeAsync();
                gameHubConnection = null;
                this.gameName = null;
                throw new Exception(joinSuccess.message);
            }
        }

        private void NotifyOfFullGameState(FullGameStateInfo fullState) {
            //Gameinfo needs to be first as client reaction depends on it
            NotifyOfGameInfo(fullState.gameInfo);
            NotifyOfClients(fullState.allClients);
            NotifyOfGameSettingsInfo(fullState.liveGameSettings);

            foreach(var charactersInScene in fullState.liveCharactersInScenesState) {
                NotifyOfCharactersInScene((charactersInScene.Key, charactersInScene.Value));
            }
        }
        
        public async Task RequestRefreshGameState(string gameName)
        {
            if (gameHubConnection == null)
            {
                throw new Exception("Game hub connection not initialized");
            }

            await gameHubConnection.InvokeAsync("RefreshGameState", gameName);
        }

        
        public ISignalRHubClientService.NotifyOfGameInfoCallback? notifyOfGameInfoCallback = null;

        public void InjectNotifyOfGameInfoDelegate(ISignalRHubClientService.NotifyOfGameInfoCallback callback)
        {
            notifyOfGameInfoCallback = callback;
        }

        // Gives the scenes so we can setup the UI, as well as any other global game info.
        // Neccesary during initial config, but may also be called at any other time.
        private void NotifyOfGameInfo(GameInfo gameInfo)
        {
            if (notifyOfGameInfoCallback is null)
            {
                Console.WriteLine("Warning! No game info notify callback! Call InjectNotifyOfGameInfoDelegate");
                return;
            }

            notifyOfGameInfoCallback(gameInfo);
        }

        public ISignalRHubClientService.NotifyOfClientsCallback? notifyOfClientsCallback = null;

        public void InjectNotifyOfClientsDelegate(ISignalRHubClientService.NotifyOfClientsCallback callback)
        {
            notifyOfClientsCallback = callback;
        }

        private void NotifyOfClients(List<ClientInfo> clients)
        {
            if (notifyOfClientsCallback is null)
            {
                Console.WriteLine("Warning! No client notify callback! Call InjectNotifyOfClientsDelegate");
                return;
            }

            notifyOfClientsCallback(clients);
        }

        public ISignalRHubClientService.NotifyOfCharactersInSceneCallback notifyOfCharactersInSceneCallback = null;

        public void InjectNotifyOfCharactersInSceneDelegate(ISignalRHubClientService.NotifyOfCharactersInSceneCallback callback)
        {
            notifyOfCharactersInSceneCallback = callback;
        }

        private void NotifyOfCharactersInScene((string, Dictionary<string, CharacterInfo>) charactersInScene) {
            if (notifyOfCharactersInSceneCallback is null)
            {
                Console.WriteLine("Warning! No characters in scene notify callback! Call InjectNotifyOfCharactersInSceneDelegate");
                return;
            }

            notifyOfCharactersInSceneCallback(charactersInScene);
        }

        public ISignalRHubClientService.NotifyOfGameSettingsInfoCallback notifyOfGameSettingsInfoCallback = null;

        public void InjectNotifyOfGameSettingsInfoDelegate(ISignalRHubClientService.NotifyOfGameSettingsInfoCallback callback)
        {
            notifyOfGameSettingsInfoCallback = callback;
        }

        private void NotifyOfGameSettingsInfo(GameSettingsInfo gameSettingsInfo)
        {
            if (notifyOfGameSettingsInfoCallback is null)
            {
                Console.WriteLine("Warning! No game settings info notify callback! Call InjectNotifyOfGameSettingsInfoDelegate");
                return;
            }

            notifyOfGameSettingsInfoCallback(gameSettingsInfo);
        }

        public ISignalRHubClientService.NotifyEventInfoCallback notifyOfEventInfoCallback = null;

        public void InjectNotifyEventInfoDelegate(ISignalRHubClientService.NotifyEventInfoCallback callback)
        {
            notifyOfEventInfoCallback = callback;
        }

        private void NotifyOfEventInfo(EventInfo eventInfo)
        {
            if (notifyOfEventInfoCallback is null)
            {
                Console.WriteLine("Warning! No event info notify callback! Call InjectNotfyEventInfoDelegate");
                return;
            }

            notifyOfEventInfoCallback(eventInfo);
        }

        public ISignalRHubClientService.NotifyOfServerErrorCallback notifyOfServerErrorCallback = null;

        public void InjectNotifyOfServerErrorDelegate(ISignalRHubClientService.NotifyOfServerErrorCallback callback)
        {
            notifyOfServerErrorCallback = callback;
        }

        private void NotifyOfServerError(string message)
        {
            if (notifyOfServerErrorCallback is null)
            {
                Console.WriteLine("Warning! No server error notify callback! Call InjectNotifyOfServerErrorDelegate");
            }

            notifyOfServerErrorCallback(message);
        }

        public async Task SendEvent(string gameName, EventInfo theEvent)
        {
            if (gameHubConnection == null)
            {
                throw new Exception("Game hub connection not initialized");
            }

            await gameHubConnection.InvokeAsync("SendEvent", gameName, theEvent);
        }

        public async Task UpdateGameSettings(string gameName, GameSettingsInfo newGameSettingsInfo)
        {
            if (gameHubConnection == null)
            {
                throw new Exception("Game hub connection not initialized");
            }

            await gameHubConnection.InvokeAsync("ChangeGameSettings", gameName, newGameSettingsInfo);
        }

        //Request the server change a client to a different scene
        public async Task ChangeClientScene(string gameName, string clientToChangeSignalRConnectionID, string newScene)
        {
            if (gameHubConnection == null)
            {
                throw new Exception("Game hub connection not initialized");
            }

            await gameHubConnection.InvokeAsync("ChangeClientScene", gameName, clientToChangeSignalRConnectionID, newScene);
        }

        public string GetConnectionID()
        {
            if (gameHubConnection == null)
            {
                throw new Exception("Game hub connection not initialized");
            }

            return gameHubConnection.ConnectionId;
        }

    }
}
