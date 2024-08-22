using Mythologist_Client_WASM.Client.Infos;

namespace Mythologist_Client_WASM.Services
{
    public interface IClientsService
    {
        public void Add(string gameName, string signalRConnectionID, string username, string? discordClientID, Uri? avatarUrl, bool isGM);
        //Return the gamename that was removed from. This is the groupname in SignalR terms
        //If the game ends up being deleted, it dosen't return
        public string? Remove(string signalRConnectionID);

        //Dictionary<signalRConnectionID, ClientInfo>
        public Dictionary<string, ClientInfo> GetClientsInGame(string gameName);
        public List<ClientInfo> GetClientsInGameAsList(string gameName, bool dontThrow = false);
        public ClientInfo GetClient(string gameName, string signalRConnectionID);
        public void SetClients(string gameName, List<ClientInfo> clients);
    }
}
