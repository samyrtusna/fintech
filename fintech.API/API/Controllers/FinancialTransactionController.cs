using fintech.API.Application.DTOs.ApiResponsesDtos;
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
        public async Task<ActionResult<ApiResponsesDto<FinancialTransactionResponseDto>>> CreateFinancialTransactionAsync(FinancialTransactionRequestDto dto)  
        {
            var result = await financialTransactionService.CreateFinancialTransactionAsync(dto, UserId);
            return CreatedAtRoute("GetTransactionById", new { id = result.Id }, result);
        }
        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponsesDto<PaginatedResult<FinancialTransactionResponseDto>>>> GetFilteredFinancialTransactionsAsync([FromQuery] FinancialTransactionFilterDto dto)  
        {
            var result = await financialTransactionService.GetFinancialTransactionsByFilterAsync(dto, UserId);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetTransactionById")]
        public async Task<ActionResult<ApiResponsesDto<FinancialTransactionResponseDto>>> GetFinancialTransactionByIdAsync(Guid id)
        {
            var result = await financialTransactionService.GetFinancialTransactionByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponsesDto<FinancialTransactionResponseDto>>> UpdateFinancialTransactionAsync(Guid id, UpdateFinancialTransactionRequestDto dto)
        {
            var result = await financialTransactionService.UpdateFinancialTransactionAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponsesDto<ConfirmationResponseDto>>> DeleteFinancialTransactionAsync(Guid id)
        {
            var result = await financialTransactionService.DeleteFinancialTransactionAsync(id, UserId);
            return Ok(result);
        }
    }
}
