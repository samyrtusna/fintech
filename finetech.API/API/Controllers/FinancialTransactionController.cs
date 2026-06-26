using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Application.DTOs.QueryDtos;
using fintech.API.Application.Interfaces.Services.IFinancialTransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class FinancialTransactionController(IFinancialTransactionCommandService transactionCommandService, IFinancialTransactionQueryService transactionQueryService) : BaseController
    {
        [HttpPost] 
        public async Task<ActionResult> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto)  
        {
            var result = await transactionCommandService.CreateFinancialTransactionAsync(dto, UserId);
            return CreatedAtAction("GetTransactionById", new { id = result.Id }, result);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<PaginatedResult<FinancialTransactionResponseDto>>> GetFilteredFinancialTransactionsAsync([FromQuery] FinancialTransactionFilterDto dto)  
        {
            var result = await transactionQueryService.GetFinancialTransactionsByFilterAsync(dto, UserId);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetTransactionById")]
        public async Task<ActionResult<FinancialTransactionResponseDto>> GetFinancialTransactionByIdAsync(Guid id)
        {
            var result = await transactionQueryService.GetFinancialTransactionByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<FinancialTransactionResponseDto>> UpdateFinancialTransactionAsync(Guid id, UpdateFinancialTransactionRequestDto dto)
        {
            var result = await transactionCommandService.UpdateFinancialTransactionAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteFinancialTransactionAsync(Guid id)
        {
            await transactionCommandService.DeleteFinancialTransactionAsync(id, UserId);
            return NoContent();
        }
    }
}
