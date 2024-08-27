using Mythologist_Client_WASM.Client.Infos;
using Mythologist_Client_WASM.Game;
using SharedLogic.Services;

namespace Mythologist_Client_WASM.Services
{
    public interface IGameRoomService
    {
       public Task NewClientConnection(string gameName, string signalRConnectionID, string username, string? discordClientID, Uri? avatarUrl, bool isGM, IDatabaseConnectionService database);
       // Return the gamename that was removed from. This is the groupname in SignalR terms
       // If the game ends up being deleted, it dosen't return
       public string? ClientDisconnection(string signalRConnectionID);
       public void UpdateSettings(string gameId, GameSettingsInfo settings);
       public GameRoom GetRoom(string gameId);
 
    }
}
