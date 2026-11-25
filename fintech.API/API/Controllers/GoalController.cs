using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.GoalDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]

    public class GoalController(IGoalService goalService) : BaseController
    {
        [HttpPost("new")]
        public async Task<ActionResult<ApiResponsesDto<GoalResponseDto>>> CreateGoalAsync([FromBody] GoalRequestDto dto)
        {
            var result = await goalService.CreateGoalsAsync(UserId, dto);
            return CreatedAtRoute("GetGoalById", new { id = result.Id }, result); ;
        }

        [HttpGet("byMonth")]
        public async Task<ActionResult<ApiResponsesDto<List<GoalResponseDto>>>> GetGoalsByMonthAsync([FromQuery] DateTime month)
        {
            var result = await goalService.GetGoalsByMonthAsync(UserId, month);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetGoalById")]
        public async Task<ActionResult<ApiResponsesDto<GoalResponseDto>>> GetGoalByIdAsync(Guid id)
        {
            var result = await goalService.GetGoalByIdAsync(UserId, id);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponsesDto<GoalResponseDto>>> UpdateGoalAsync(Guid id, [FromBody] GoalUpdateDto dto)
        {
            var result = await goalService.UpdateGoalAsync(UserId, id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponsesDto<GoalResponseDto>>> DeleteGoalAsync(Guid id)
        {
            var result = await goalService.DeleteGoalAsync(UserId, id);
            return Ok(result);
        }
    }
}
