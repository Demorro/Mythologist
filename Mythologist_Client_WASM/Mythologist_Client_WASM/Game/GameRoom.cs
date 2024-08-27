using Mythologist_Client_WASM.Client.Infos;
using SharedLogic.Services;

namespace Mythologist_Client_WASM.Game
{
    public class GameRoom
    {
        public static async Task<GameRoom> CreateGameRoom(string gameId, IDatabaseConnectionService database) {
            var characters = await CharacterSceneState.CreateCharacterSceneState(gameId, database);
            return new GameRoom(gameId, characters);
        }

        private GameRoom(string _gameId, CharacterSceneState _characterSceneState) {
            gameID = _gameId;
            characterSceneState = _characterSceneState;
        }

        public string gameID {get; private set; }
        public GameSettingsInfo liveGameSettings {get; private set;} = new GameSettingsInfo();

        // <signalRConnectionID, ClientState>
        private Dictionary<string, ClientInfo> connectedClients = new Dictionary<string, ClientInfo>();
        private CharacterSceneState characterSceneState;

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

        public CharacterSceneState GetCharacters() {
            return characterSceneState;
        }
    }
}
