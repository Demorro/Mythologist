using Mythologist_Client_WASM.Client.Infos;
using System.Collections.Generic;
using System.Security.Policy;

namespace Mythologist_Client_WASM.Services
{
    public class ClientsService : IClientsService
    {
        // Dictionary<gameName, Dictionary<signalRConnectionID, ClientInfo>>
        private Dictionary<string, Dictionary<string,ClientInfo>> clients = new Dictionary<string, Dictionary<string,ClientInfo>>();
        public void Add(string gameName, string signalRConnectionID, string username, string? discordClientID, Uri? avatarUrl, bool isGM)
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

            lock (clients)
            {
                if (!clients.ContainsKey(gameName))
                {
                    clients[gameName] = new Dictionary<string, ClientInfo>();
                }

                clients[gameName][signalRConnectionID] = clientInfo;
            }
        }

        public string? Remove(string signalRConnectionID)
        {
            //Slow. Could redo this so that we keep a full dictionary of all the clients in sync for a direct reference
            //Loop over every game looking for the client

            string? gameRemovedFrom = null;
            foreach(var game in clients)
            {
                if (game.Value.ContainsKey(signalRConnectionID))
                {
                    lock (clients)
                    {
                        gameRemovedFrom = game.Key;
                        game.Value.Remove(signalRConnectionID);
                        if (game.Value.Count == 0)
                        {
                            clients.Remove(game.Key);
                            gameRemovedFrom = null;
                        }
                    }
                }
            }

            return gameRemovedFrom;
         }

        public Dictionary<string, ClientInfo> GetClientsInGame(string gameName)
        {
            if (!clients.ContainsKey(gameName))
            {
                throw new Exception($"Game {gameName} does not exist");
            }

            return clients[gameName];
        }

        public List<ClientInfo> GetClientsInGameAsList(string gameName, bool dontThrow)
        {
            if (!clients.ContainsKey(gameName))
            {
                if (dontThrow)
                {
                    return new List<ClientInfo>();
                }
                throw new Exception($"Game {gameName} does not exist");
            }

            return clients[gameName].Values.ToList();
        }

        public ClientInfo GetClient(string gameName, string signalRConnectionID)
        {
            if (clients.ContainsKey(gameName))
            {
                if (clients[gameName].ContainsKey(signalRConnectionID))
                {
                    return clients[gameName][signalRConnectionID];
                }
            }

            throw new Exception($"Could not find client with connectionID {signalRConnectionID} in game {gameName}");
        }

        public void SetClients(string gameName, List<ClientInfo> newClients)
        {
            lock (clients[gameName])
            {
                if (!clients.ContainsKey(gameName))
                {
                    clients[gameName] = new Dictionary<string, ClientInfo>();
                }

                foreach (ClientInfo clientInfo in newClients)
                {
                    clients[gameName].Add(clientInfo.signalRConnectionID, clientInfo);
                }
            }
        }
    }
}
