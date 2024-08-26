using SharedLogic.Model;

namespace Mythologist_Client_WASM.Client.Infos
{
    public class GameInfo
    {
        public List<SceneModel> scenes { get; set; }

        public GameSettingsModel gameSettings { get; set; }

        public List<CharacterModel> characters {get; set; }

        public List<string> SceneNames()
        {
            return scenes.Select(x => x.id).ToList();
        }
    }
}
