using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class ExchangeRateApiController(IExchangeRateApiService exchangeRateApiService) : BaseController
    {
        [HttpGet("symbols")]
        public async Task<ActionResult<IEnumerable<string>>> GetExchangeRateSymbolsAsync()
        {
            var symbols = await exchangeRateApiService.GetSymbolsAsync();
            return Ok(symbols);
        }
    }
}
