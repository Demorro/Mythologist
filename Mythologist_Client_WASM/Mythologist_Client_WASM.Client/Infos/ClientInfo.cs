namespace Mythologist_Client_WASM.Client.Infos
{
    public class ClientInfo
    {
        public bool isGM { get; set; }
        public string userName { get; set; }
        public string signalRConnectionID { get; set; }
        public string? discordClientID { get; set; }
        public Uri? avatarUrl { get; set; }
        public string? currentSceneID {  get; set; }
    }
}
