using fintech.API.Application.DTOs.ApiResponsesDtos;
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
        [HttpPost]
        public async Task<ActionResult<ApiResponsesDto<ConfirmationResponseDto>>> CreateMonthlyAggregates([FromBody]DateTime date) {
            var results = await financialAggregatesService.CreateMonthlyAggregatesAsync(UserId, date);
            return Ok(results);
        }

        [HttpGet("month")]
        public async Task<ActionResult<ApiResponsesDto<FinancialAggregatesResponseDto>>> GetMonthlyAggregates([FromQuery]DateTime date)
        {
            var results = await financialAggregatesService.GetMonthlyAggregatesAsync(UserId, date);
            return Ok(results);
        }

        [HttpGet("global")]
        public async Task<ActionResult<ApiResponsesDto<List<FinancialAggregateDto>>>> GetGlobalAggregates([FromQuery] DateTime date)
        {
            var results = await financialAggregatesService.GetGlobalAggregatesAsync(UserId, date);
            return Ok(results);
        }
    }
}
