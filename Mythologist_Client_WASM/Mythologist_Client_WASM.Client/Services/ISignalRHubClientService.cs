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

        public delegate void NotifyOfGameSettingsInfoCallback(GameSettingsInfo gameSettingsInfo);
        public void InjectNotifyOfGameSettingsInfoDelegate(NotifyOfGameSettingsInfoCallback callback);

        public delegate void NotifyEventInfoCallback(EventInfo eventInfo);
        public void InjectNotifyEventInfoDelegate(NotifyEventInfoCallback callback);

        public delegate void NotifyOfServerErrorCallback(string message);
        public void InjectNotifyOfServerErrorDelegate(NotifyOfServerErrorCallback callback);

        //Actual real logic

        public Task SendEvent(string gameName, EventInfo theEvent);

        //Request the server update the game settings
        public Task UpdateGameSettings(string gameName, GameSettingsInfo newGameSettingsInfo);

        //Request the server change a specific client scene.
        public Task ChangeClientScene(string gameName, string clientToChangeSignalRConnectionID, string newScene);

        public string GetConnectionID();

    }
}
