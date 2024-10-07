using Mythologist_Client_WASM.Client.Infos;
using SharedLogic.Model;
using System.Threading.Tasks;

namespace Mythologist_Client_WASM.Client.Services
{
    public interface ISignalRHubClientService
    {
        public Task StopAnyConnections();
        public Task InitializeConnectionAndJoinGame(string gameName, string userName, Utils.DiscordUser? discordUserObject, string? GMPassword);

        public Task RequestRefreshGameState(string gameName);

        public delegate void NotifyOfGameInfoCallback(GameInfo gameInfo);
        public void InjectNotifyOfGameInfoDelegate(NotifyOfGameInfoCallback callback);

        public delegate void NotifyOfClientsCallback(List<ClientInfo> clients);
        public void InjectNotifyOfClientsDelegate(NotifyOfClientsCallback callback);

         //(sceneId, <characterID, Character>)
        public delegate void NotifyOfCharactersInSceneCallback((string, Dictionary<string, CharacterInfo>) charactersInScene);
        public void InjectNotifyOfCharactersInSceneDelegate(NotifyOfCharactersInSceneCallback callback);

        public delegate void NotifyOfGameSettingsInfoCallback(GameSettingsInfo gameSettingsInfo);
        public void InjectNotifyOfGameSettingsInfoDelegate(NotifyOfGameSettingsInfoCallback callback);

        public delegate void NotifyEventCallback(Event Event);
        public void InjectNotifyEventDelegate(NotifyEventCallback callback);

        public delegate void NotifyOfServerErrorCallback(string message);
        public void InjectNotifyOfServerErrorDelegate(NotifyOfServerErrorCallback callback);


        //Actual real logic
        public Task SendEvent(string gameName, Event theEvent);

        //Notify the server to update the game settings
        public Task SendUpdateGameSettings(string gameName, GameSettingsInfo newGameSettingsInfo);

        //Notify the server to change a specific client scene.
        public Task SendChangeClientScene(string gameName, string clientToChangeSignalRConnectionID, string newScene);

        //Notify the server to change the character state of a scene
        public Task SendUpdateCharacterState(string gameName, CharacterInfo characterUpdate);





        public string GetConnectionID();

    }
}
