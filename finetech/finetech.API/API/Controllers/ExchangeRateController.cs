using fintech.API.Application.DTOs.ExchangeRateDtos;
using fintech.API.Application.Interfaces.Services.IExchangeRateServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class ExchangeRateController(IExchangeRateCommandService commandService, IExchangeRateQueryService queryService) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateExchangeRateAsync([FromBody] CreateExchangeRateRequestDto dto)
        {
            var result = await commandService.CreateExchangeRateAsync(dto);

            return CreatedAtAction("GetExchangeRate", new { id = result.Id }, result);
        }

        [HttpGet("{id}", Name ="GetExchangeRate")]
        public async Task<ActionResult<ExchangeRateResponseDto>> GetExchangeRateAsync (Guid id)
        {
            var result = await queryService.GetExchangeRateByIdAsync(id);
             
            return Ok(result);
        }
    }
}
