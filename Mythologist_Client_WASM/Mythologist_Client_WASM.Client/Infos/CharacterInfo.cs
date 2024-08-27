using SharedLogic.Model;

namespace Mythologist_Client_WASM.Client.Infos
{
    public class CharacterInfo
    {
        // So the thing about this is that characters are more than this, (for example, no bio).
        // Whatsmore, characters can exist in multiple scenes. In that case you'd have duplicate CharacterInfos, (with different scenes),
        // but not duplicate CharacterModels (DB Objects). The idea is that these infos can be sent a lot as they're small.
        // We'll be sending infos when the GM makes a character enter/exit a scene for example.
        // Stuff in info's is session specific, and dosen't persist.
        public string id { get; set; }
        public string scene {get; set;}
        public bool hasEnteredScene {get; set;}
    }
}
