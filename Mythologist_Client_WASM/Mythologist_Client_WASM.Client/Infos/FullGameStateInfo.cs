namespace Mythologist_Client_WASM.Client.Infos
{
    public class FullGameStateInfo
    {
        public GameInfo gameInfo {get; set; }

        public List<ClientInfo> allClients {get; set; }

        public Dictionary<string, Dictionary<string, CharacterInfo>> liveCharactersInScenesState {get; set; }

        public GameSettingsInfo liveGameSettings {get; set;}
    }
}
