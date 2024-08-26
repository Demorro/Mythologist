namespace Mythologist_Client_WASM.Client.Infos
{
    public class FullGameStateInfo
    {
        public GameInfo gameInfo {get; set; }

        public List<ClientInfo> allClients {get; set; }

        public List<CharacterInfo> allCharacters {get; set; }
    }
}
