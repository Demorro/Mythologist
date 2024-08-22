using Mythologist_Client_WASM.Client.Infos;

namespace Mythologist_Client_WASM.Services
{
    //A service for softly storing gamestate.
    //This isn't in an authoratitive way as such, its more like, when people join, they'll need to know what the current gamestate is for things like settings.
    //Try to avoid using this for things that should just be tracked on an eventwise basis, like which rooms players are in
    public interface ILastKnownStateService
    {
        public GameSettingsInfo LastKnownSettings();
        public void SetLastKnownSettings(GameSettingsInfo settings);
    }
}
