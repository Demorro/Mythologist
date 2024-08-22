using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using System;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Mythologist_ContentServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public ProxyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet("fileget/{**catchAll}")]
        public async Task<IActionResult> Get(string catchAll)
        {
            Uri targetUrl = new Uri(WebUtility.UrlDecode(catchAll));

  
            // Send a GET request to the specified URL
            HttpResponseMessage response = await _httpClient.GetAsync(targetUrl);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsByteArrayAsync();


            return File(responseBody, response.Content.Headers.ContentType?.ToString());
        }

    }
}
