using SharedLogic.Model;

namespace Mythologist_Client_WASM.Client.Infos
{
    public class SceneChangeInfo
    {
        public string newSceneId;

        //So when you join the scene, you get to _vaguely_ the same point in the music track as everyone else.
        public float timeSinceMusicStart;
    }
}
