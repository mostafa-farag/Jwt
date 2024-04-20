using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CoinsKeyController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public CoinsKeyController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetLatestCryptocurrencyListings()
        {
            var client = _clientFactory.CreateClient();

            // Add your CoinMarketCap API key to the request headers
            client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", "b4c83476-983a-48b2-8df4-ee16a2dba3ac");

            var request = new HttpRequestMessage(HttpMethod.Get, "https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest");

            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                // You can parse and process the response content here
                return Ok(content);
            }
            else
            {
                // Handle error cases
                return StatusCode((int)response.StatusCode, response.ReasonPhrase);
            }
        }
    }

}
