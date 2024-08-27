using Mythologist_Client_WASM.Client.Infos;
using Mythologist_Client_WASM.Game;
using SharedLogic.Services;

namespace Mythologist_Client_WASM.Services
{
    public class GameRoomService : IGameRoomService
    {
        //<gameName, GameRoom>
        private Dictionary<string, GameRoom> rooms = new Dictionary<string, GameRoom>();

        public async Task NewClientConnection(string gameName, string signalRConnectionID, string username, string? discordClientID, Uri? avatarUrl, bool isGM, IDatabaseConnectionService database)
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


            //We always make a new room, but it dosen't get added unless there isn't already one.
            //(We cant await inside a lock, so we take a redundant hit to the DB here a lot of the time ... maybe a better way of doing this)
            GameRoom? newRoom =  await GameRoom.CreateGameRoom(gameName, database);
            lock (rooms)
            {
                if (!rooms.ContainsKey(gameName))
                {
                    rooms[gameName] = newRoom;
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
