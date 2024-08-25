using Microsoft.AspNetCore.Mvc;
using Mythologist_Client_WASM.Services;

namespace Mythologist_Client_WASM.Api
{
    [ApiController]
    [Route("api/discordtoken")]
    public class DiscordTokenController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public DiscordTokenController(IConfiguration _configuration, HttpClient _httpClient)
        {
            configuration = _configuration;
            httpClient = _httpClient;
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
        {
            var clientId = configuration["DiscordClientID"];
            var clientSecret = configuration["DiscordClientSecret"];

            var parameters = new Dictionary<string, string>
            {
                { "client_id", clientId! },
                { "client_secret", clientSecret! },
                { "grant_type", "authorization_code" },
                { "code", request.Code! }
            };

            var content = new FormUrlEncodedContent(parameters);

            var response = await httpClient.PostAsync("https://discord.com/api/oauth2/token", content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, responseString);
            }

            var tokenResponse = System.Text.Json.JsonSerializer.Deserialize<TokenResponse>(responseString);

            if (tokenResponse == null) {
                return BadRequest(new { error = "Invalid token response" });
            }

            return Ok(new { access_token = tokenResponse.access_token });
        }

        public class TokenRequest
        {
            public string? Code { get; set; }
        }

        public class TokenResponse
        {
            public string? access_token { get; set; }
        }
    }
}
