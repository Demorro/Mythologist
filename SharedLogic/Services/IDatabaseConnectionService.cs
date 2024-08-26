using SharedLogic.Model;

namespace SharedLogic.Services
{
    public interface IDatabaseConnectionService
    {
        Task CreateNewGame(string gameName, string GMPassword);
        Task<bool> VerifyGameExists(string gameName);
        Task<bool> VerifyLogin(string gameName, string GMPassword);


        Task<List<SceneModel>> AllScenes(string gameName);
        Task<SceneModel?> Scene(string gameName, string sceneID);
        Task AddScene(string gameName, SceneModel newScene);
        Task RenameScene(string gameName, SceneModel oldScene, string newSceneName);
        Task UpdateScene(string gameName, SceneModel updatedScene);
        Task RemoveScene(string gameName, SceneModel sceneToDelete);


        Task<List<CharacterModel>> AllCharacters(string gameName);
        Task<CharacterModel> Character(string gameName, string characterID);
        Task AddCharacter(string gameName, CharacterModel newCharacter);
        Task RenameCharacter(string gameName, CharacterModel oldCharacter, string newCharacterName);
        Task UpdateCharacter(string gameName, CharacterModel updatedCharacter);
        Task RemoveCharacter(string gameName, CharacterModel characterToDelete);


        Task UpdateGameSettings(string gameName, GameSettingsModel settings);
        Task<GameSettingsModel> GameSettings(string gameName);
        Task<Guid> StorageGuid(string gameName);

	}
}
