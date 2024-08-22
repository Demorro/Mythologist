using SharedLogic.Model;

namespace SharedLogic.Services
{
    public interface IDatabaseConnectionService
    {
        List<SceneModel> AllScenes();
        SceneModel? Scene(string sceneID);
        Task HydrateForGame(string gameName);
        Task CreateNewGame(string gameName, string GMPassword);
        Task<bool> VerifyGameExists(string gameName);
        Task<bool> VerifyLogin(string gameName, string GMPassword);
        Task AddScene(string gameName, SceneModel newScene);
        Task RenameScene(string gameName, SceneModel oldScene, string newSceneName);
        Task UpdateScene(string gameName, SceneModel updatedScene);
        Task RemoveScene(string gameName, SceneModel sceneToDelete);
        Task UpdateGameSettings(string gameName, GameSettingsModel settings);

        Task<GameSettingsModel> GameSettings(string gameName);

	}
}
