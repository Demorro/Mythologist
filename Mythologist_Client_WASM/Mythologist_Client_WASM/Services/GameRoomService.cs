using Mythologist_Client_WASM.Client.Infos;
using Mythologist_Client_WASM.Game;

namespace Mythologist_Client_WASM.Services
{
    public class GameRoomService : IGameRoomService
    {
        //<gameName, GameRoom>
        private Dictionary<string, GameRoom> rooms = new Dictionary<string, GameRoom>();

        public void NewClientConnection(string gameName, string signalRConnectionID, string username, string? discordClientID, Uri? avatarUrl, bool isGM)
        {
            ClientInfo clientInfo = new ClientInfo
            {
                isGM = isGM,
                userName = username,
                signalRConnectionID = signalRConnectionID,
                discordClientID = discordClientID,
                avatarUrl = avatarUrl,
                currentSceneID = null
            };

            lock (rooms)
            {
                if (!rooms.ContainsKey(gameName))
                {
                    rooms[gameName] = new GameRoom(gameName);
                }

                rooms[gameName].AddClient(clientInfo);
            }
        }

        //Returns the name of the game the client was removed from
        public string? ClientDisconnection(string signalRConnectionID)
        {
            //Slow. Could redo this so that we keep a full dictionary of all the clients in sync for a direct reference
            //Loop over every game looking for the client

            string? gameRemovedFrom = null;
            foreach(var game in rooms.Values)
            {
                if (game.HasClientBySignalRID(signalRConnectionID))
                {
                    lock (rooms)
                    {
                        gameRemovedFrom = game.gameID;
                        game.RemoveClient(signalRConnectionID);
                        if (game.ClientCount() == 0)
                        {
                            rooms.Remove(game.gameID);
                            gameRemovedFrom = null;
                        }
                    }
                }
            }

            return gameRemovedFrom;
        }

        public void UpdateSettings(string gameId, GameSettingsInfo settings) {
             if (!rooms.ContainsKey(gameId)) {
                 throw new Exception($"Game '{gameId}' does not exist");
            }

             rooms[gameId].UpdateSettings(settings);
        }

        public GameRoom GetRoom(string gameId) {
            if (!rooms.ContainsKey(gameId)) {
                 throw new Exception($"Game '{gameId}' does not exist");
            }

            return rooms[gameId];
        }

    }
}
