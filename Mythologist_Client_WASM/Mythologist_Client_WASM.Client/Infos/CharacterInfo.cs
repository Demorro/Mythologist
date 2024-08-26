using SharedLogic.Model;

namespace Mythologist_Client_WASM.Client.Infos
{
    public class CharacterInfo
    {
        public bool visibleToPlayers {get; set;} = true;
        public string? currentSceneID {  get; set; }
        public CharacterModel characterModel {get; set; }
    }
}
