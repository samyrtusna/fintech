using fintech.Application.DTOs.TransactionDtos;
using fintech.Application.Interfaces.Services.ITransactionServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class TransactionController(ITransactionCommandService transactionCommandService, ITransactionQueryService transactionQueryService) : BaseController
    {
        [HttpPost] 
        public async Task<ActionResult> CreateTransactionAsync(TransactionRequestDto dto)
        {
            var result = await transactionCommandService.CreateTransactionAsync(dto, UserId);
            return CreatedAtAction(nameof(GetTransactionByIdAsync), new { id = result.Id }, result);
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponseDto>>> GetFilteredTransactionsAsync(TransactionFilterDto dto)
        {
            var result = await transactionQueryService.GetTransactionsByFilterAsync(dto, UserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> GetTransactionByIdAsync(Guid id)
        {
            var result = await transactionQueryService.GetTransactionByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponseDto>> UpdateTransactionAsync(Guid id, UpdateTransactionRequestDto dto)
        {
            var result = await transactionCommandService.UpdateTransactionAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTransactionAsync(Guid id)
        {
            await transactionCommandService.DeleteTransactionAsync(id, UserId);
            return NoContent();
        }
    }
}
