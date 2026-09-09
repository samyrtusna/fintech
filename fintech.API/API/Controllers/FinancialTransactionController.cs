using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class FinancialTransactionController(IFinancialTransactionService financialTransactionService) : BaseController
    {
        [HttpPost] 
        public async Task<ActionResult> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto)  
        {
            var result = await financialTransactionService.CreateFinancialTransactionAsync(dto, UserId);
            return CreatedAtRoute("GetTransactionById", new { id = result.Id }, result);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<PaginatedResult<FinancialTransactionResponseDto>>> GetFilteredFinancialTransactionsAsync([FromQuery] FinancialTransactionFilterDto dto)  
        {
            var result = await financialTransactionService.GetFinancialTransactionsByFilterAsync(dto, UserId);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetTransactionById")]
        public async Task<ActionResult<FinancialTransactionResponseDto>> GetFinancialTransactionByIdAsync(Guid id)
        {
            var result = await financialTransactionService.GetFinancialTransactionByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FinancialTransactionResponseDto>> UpdateFinancialTransactionAsync(Guid id, UpdateFinancialTransactionRequestDto dto)
        {
            var result = await financialTransactionService.UpdateFinancialTransactionAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFinancialTransactionAsync(Guid id)
        {
            await financialTransactionService.DeleteFinancialTransactionAsync(id, UserId);
            return NoContent();
        }
    }
}
