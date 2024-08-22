using Mythologist_Client_WASM.Client.Utils;

namespace Mythologist_Client_WASM.Client.Services
{
    public interface IDiscordApiService
    {
        public Task<string> GetAccessToken();
        public Task<DiscordUser> GetUserObject();
    }
}
