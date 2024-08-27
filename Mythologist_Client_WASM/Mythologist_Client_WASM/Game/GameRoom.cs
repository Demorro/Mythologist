using Mythologist_Client_WASM.Client.Infos;

namespace Mythologist_Client_WASM.Game
{
    public class GameRoom
    {
        public GameRoom(string id) {
            gameID = id;
        }

        public string gameID {get; private set; }
        public GameSettingsInfo liveGameSettings {get; private set;} = new GameSettingsInfo();

        // <signalRConnectionID, ClientState>
        private Dictionary<string, ClientInfo> connectedClients = new Dictionary<string, ClientInfo>();
        // <characterID, CharacterState>
        private Dictionary<string, CharacterInfo> charactersInScene = new Dictionary<string, CharacterInfo>();

        public void AddClient(ClientInfo client) {
            connectedClients.TryAdd(client.signalRConnectionID, client);
        }

        public void RemoveClient(string signalRConnectionID) {
            connectedClients.Remove(signalRConnectionID);
        }

        public void UpdateSettings(GameSettingsInfo settings) {
            liveGameSettings = settings;
        }

        public int ClientCount() {
            return connectedClients.Count;
        }

        public bool HasClientBySignalRID(string signalRConnectionID) {
            return connectedClients.ContainsKey(signalRConnectionID);
        }

        public List<ClientInfo> GetClientsInGameAsList() {
            return connectedClients.Values.ToList();
        }

        public Dictionary<string, ClientInfo> GetClientsInGameAsDict() {
            return connectedClients;
        }

        public ClientInfo GetClient(string signalRConnectionID) {
            if (!connectedClients.ContainsKey(signalRConnectionID)){
                throw new Exception($"Could not find client with connectionID '{signalRConnectionID}' in game '{gameID}'");
            }
            return connectedClients[signalRConnectionID];
        }
    }
}
