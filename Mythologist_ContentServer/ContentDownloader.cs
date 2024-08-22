using System.Drawing.Imaging;
using System.Drawing;
using System.Net;
using System.Security.Policy;
using System;
using SharedLogic.Model;

namespace Mythologist_ContentServer
{
    public class ContentDownloader
    {
        private Dictionary<Uri, byte[]> imageCache = new Dictionary<Uri, byte[]>();
        private Dictionary<Uri, byte[]> audioCache = new Dictionary<Uri, byte[]>();

        public async Task<SceneDataModel> ToSceneDataModel(SceneModel sceneModel)
        {
            SceneDataModel sceneDataModel = new SceneDataModel(sceneModel.id);

            sceneDataModel.backgroundImageFileType = Path.GetExtension(sceneModel.backgroundImageUri?.AbsolutePath);
            sceneDataModel.backgroundMusicFileType = Path.GetExtension(sceneModel.backgroundMusicUri?.AbsolutePath);
            sceneDataModel.backgroundImage = await GetImage(sceneModel.backgroundImageUri);
            sceneDataModel.backgroundMusic = await GetAudio(sceneModel.backgroundMusicUri);
            return sceneDataModel;
        }

        public async Task<byte[]?> GetImage(Uri? address)
        {
            if (address == null) return null;

            if (imageCache.ContainsKey(address))
            {
                return imageCache[address];
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] data = await client.GetByteArrayAsync(address);
                    imageCache[address] = data;
                    return imageCache[address];
                }
                catch
                {
                    return null;
                }
            }
        }

        public async Task<byte[]?> GetAudio(Uri? address)
        {
            if (address == null) return null;

            if (audioCache.ContainsKey(address))
            {
                return audioCache[address];
            }

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    byte[] data = await client.GetByteArrayAsync(address);
                    audioCache[address] = data;
                    return audioCache[address];
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
