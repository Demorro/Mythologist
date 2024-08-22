using Microsoft.AspNetCore.Mvc;
using SharedLogic.Services;
using SharedLogic.Model;

namespace Mythologist_ContentServer.Controllers
{
    [ApiController]
    [Route("api/scenes")]
    public class SceneModelController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private DatabaseConnectionService dbConnection;
        private ContentDownloader contentDownloader = new ContentDownloader();

        public SceneModelController(IConfiguration configuration)
        {
            _configuration = configuration;
            dbConnection = new DatabaseConnectionService(configuration);
        }

        [HttpGet("{gameName}")]
        public async Task<IEnumerable<SceneModel>> Get(string gameName)
        {
            await dbConnection.HydrateForGame(gameName);
            var scenes = dbConnection.AllScenes();
            return scenes;
        }

        [HttpGet("{gameName}/{sceneName}")]
        public async Task<SceneDataModel> Get(string gameName, string sceneName)
        {
            await dbConnection.HydrateForGame(gameName);
            var scenes = dbConnection.AllScenes();
            SceneModel? sceneModel = scenes.Find(x => x.id == sceneName);

            if (sceneModel == null)
            {
                throw new Exception($"Could not access scene {sceneName} from game {gameName}");
            }

            return await contentDownloader.ToSceneDataModel(sceneModel);
        }
    }
}
