using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.Interfaces.Services.ICategoryServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    public class CategoryController(ICategoryCommandService categoryCommandService, ICategoryQueryService categoryQueryService) : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategoryAsync(CategoryRequestDto dto)
        {
            var result = await categoryCommandService.CreateCategoryAsync(dto, UserId);

            return CreatedAtAction("GetCategoryById", new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync() 
        {
            var result = await categoryQueryService.GetAllCategoriesAsync(UserId);
            return Ok(result);
        }

        [HttpGet("{id}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid id) 
        {
            var result = await categoryQueryService.GetCategoryByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPost("{id}")]
        public async Task<ActionResult<ConfirmationResponseDto>> SetCategoryEssentialAsync(Guid id,UserCategorySettingsRequestDto dto) 
        {
            var result = await categoryCommandService.SetCategoryEssentialAsync(dto,id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto dto)
        {
            var result = await categoryCommandService.UpdateCategoryAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(Guid id)
        {
            await categoryCommandService.DeleteCategoryAsync(id, UserId);
            return NoContent();
        }
    }
}
