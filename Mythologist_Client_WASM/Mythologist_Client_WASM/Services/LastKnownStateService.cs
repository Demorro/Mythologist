using Mythologist_Client_WASM.Client.Infos;

namespace Mythologist_Client_WASM.Services
{
    public class LastKnownStateService : ILastKnownStateService
    {
        private GameSettingsInfo lastKnownSettings = new GameSettingsInfo();

        public GameSettingsInfo LastKnownSettings()
        {
            return lastKnownSettings;
        }
        public void SetLastKnownSettings(GameSettingsInfo settings)
        {
            lastKnownSettings = settings;
        }
    }
}
