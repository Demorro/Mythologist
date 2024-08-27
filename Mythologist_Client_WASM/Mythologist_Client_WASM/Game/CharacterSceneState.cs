using Mythologist_Client_WASM.Client.Infos;
using SharedLogic.Model;
using SharedLogic.Services;

namespace Mythologist_Client_WASM.Game
{
    public class CharacterSceneState
    {
        public string gameId {get; private set;}
        public List<SceneModel> allScenes {get; private set;}

        // <sceneID<characterID, CharacterState>>
        private Dictionary<string, Dictionary<string, CharacterInfo>> charactersInScenes = new Dictionary<string, Dictionary<string, CharacterInfo>>();

        private CharacterSceneState(string _gameId, List<SceneModel> _allScenes) {
            this.gameId = _gameId;
            this.allScenes = _allScenes;

            //Populate the characters in scene array
            foreach(SceneModel scene in allScenes) {
                charactersInScenes.TryAdd(scene.id, new Dictionary<string, CharacterInfo>());
                foreach (string characterId in scene.charactersIdsInScene) {
                    charactersInScenes[scene.id].TryAdd(characterId, new CharacterInfo {
                        id = characterId,
                        scene = scene.id,
                        hasEnteredScene = false //TODO. Get this from some config
                    });
                }
            }
        }

        public static async Task<CharacterSceneState> CreateCharacterSceneState(string gameId, IDatabaseConnectionService database) {
            var allScenes = await database.AllScenes(gameId);
            return new CharacterSceneState(gameId, allScenes);
        }

        public void UpdateCharacter(string sceneId, CharacterInfo newCharacterInfo) {
            if (!charactersInScenes.ContainsKey(sceneId)) {
                throw new Exception($"Scene '{sceneId}' could not be found when attempting to update character");
            }
            if (!charactersInScenes[sceneId].ContainsKey(newCharacterInfo.id)) {
                throw new Exception($"Character '{newCharacterInfo.id}' does not exist in scene '{sceneId}'");
            }

            charactersInScenes[sceneId][newCharacterInfo.id] = newCharacterInfo;
        }
        
        public List<CharacterInfo> GetCharacterInfosInSceneAsList(string sceneId) {
            if (!charactersInScenes.ContainsKey(sceneId)) {
                throw new Exception($"Scene '{sceneId}' could not be found when attempting to get characters");
            }

            return charactersInScenes[sceneId].Values.ToList();
        }

        public Dictionary<string, CharacterInfo> GetCharacterInfosInSceneAsDict(string sceneId) {
            if (!charactersInScenes.ContainsKey(sceneId)) {
                throw new Exception($"Scene '{sceneId}' could not be found when attempting to get characters");
            }

            return charactersInScenes[sceneId];
        }

        public Dictionary<string, Dictionary<string, CharacterInfo>> GetAllCharactersAsDict() {
            return charactersInScenes;
        }

    }
}
