using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class FinancialAggregatesController(IFinancialAggregatesService financialAggregatesService) : BaseController
    {
        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalAggregates()
        {
            var results = await financialAggregatesService.GetGlobalAggregates(UserId);
            return Ok(results);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredAggregates([FromQuery] FinancialAggregatesFilterDto dto)
        {
            var results = await financialAggregatesService.GetAggregatesByFilter(UserId, dto);
            return Ok(results);
        }

    }
}
