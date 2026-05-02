using fintech.Application.DTOs.ApiResponsesDtos;
using fintech.Application.DTOs.CategoryDtos;
using fintech.Domain.Entities;

namespace fintech.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto dto, Guid userId);
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(Guid userId);
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid categoryId, Guid userId);
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid CategoryId, UpdateCategoryRequestDto dto, Guid userId);
        Task DeleteCategoryAsync(Guid categoryId, Guid userId);  
        Task<ConfirmationResponseDto> SetCategoryEssentialAsync(UserCategorySettingsRequestDto dto, Guid categoryId, Guid userId);
    }
}
    