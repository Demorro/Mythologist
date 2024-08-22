
using Microsoft.JSInterop;
using Mythologist_Client_WASM.Client.Utils;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Mythologist_Client_WASM.Client.Services
{
    public class DiscordApiService : IDiscordApiService
    {
        private HttpClient httpClient;
        private IJSRuntime jsRuntime;

        public DiscordApiService(HttpClient _httpClient, IJSRuntime _jsRuntime)
        {
            httpClient = _httpClient;
            jsRuntime = _jsRuntime;
        }

        public async Task<string> GetAccessToken()
        {
            return await jsRuntime.InvokeAsync<string>("getDiscordAccessToken");
        }

        public async Task<DiscordUser> GetUserObject()
        {
            DiscordUser discordUser = await jsRuntime.InvokeAsync<DiscordUser>("getDiscordUserInfo");
            //var discordUser = JsonSerializer.Deserialize<DiscordUser>(jsonString);
            if (discordUser == null)
            {
                throw new Exception("Failed to get Discord User Information");
            }
            return discordUser;
		}
    }
}
