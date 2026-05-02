using fintech.Application.DTOs.ApiResponsesDtos;
using fintech.Application.DTOs.CategoryDtos;
using fintech.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "UserPolicy")]
    public class CategoriesController(ICategoryService categoryService) : BaseController
    {
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> CreateCategoryAsync(CategoryRequestDto dto)
        {
            var result = await categoryService.CreateCategoryAsync(dto, UserId);
            
            return CreatedAtAction(nameof(GetCategoryByIdAsync), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAllCategoriesAsync() 
        {
            var result = await categoryService.GetAllCategoriesAsync(UserId);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetCategoryByIdAsync(Guid id) 
        {
            var result = await categoryService.GetCategoryByIdAsync(id, UserId);
            return Ok(result);
        }

        [HttpPatch("{id}/essential")]
        public async Task<ActionResult<ConfirmationResponseDto>> SetCategoryEssentialAsync(Guid id,UserCategorySettingsRequestDto dto) 
        {
            var result = await categoryService.SetCategoryEssentialAsync(dto,id, UserId);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> UpdateCategoryAsync(Guid id, UpdateCategoryRequestDto dto)
        {
            var result = await categoryService.UpdateCategoryAsync(id, dto, UserId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCategoryAsync(Guid id)
        {
            await categoryService.DeleteCategoryAsync(id, UserId);
            return NoContent();
        }
    }
}
