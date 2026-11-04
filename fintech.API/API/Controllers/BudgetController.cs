using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.BudgetDtos;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BudgetController(IBudgetService budgetService) : BaseController
    {
        [HttpPost("new")]
        public async Task<ActionResult<ApiResponsesDto<BudgetResponseDto>>> CreateNewBudgetAsync(BudgetRequestDto dto)
        {
            var result = await budgetService.CreateNewBudgetAsync(UserId, dto);
            return CreatedAtAction(nameof(GetBudgetByIdAsync), new { id = result.Id }, result);
        }

        [HttpGet("month")]
        public async Task<ActionResult<ApiResponsesDto<List<BudgetResponseDto>>>> GetBudgetsByMonthAsync(DateTime month)
        {
            var result = await budgetService.GetBudgetsByMonthAsync(UserId, month);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponsesDto<BudgetResponseDto>>> GetBudgetByIdAsync(Guid id)
        {
            var result = await budgetService.GetBudgetByIdAsync(UserId, id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponsesDto<BudgetResponseDto>>> UpdateBudgetAsync(Guid id, Money amount)
        {
            var result = await budgetService.UpdateBudgetAsync(UserId, id, amount);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponsesDto<ConfirmationResponseDto>>> DeleteBudgetAsync(Guid id)
        {
            var result = await budgetService.DeleteBudgetAsync(UserId, id);
            return Ok(result);
        }
    }
}
