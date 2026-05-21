using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.CategoryDtos;


namespace fintech.API.Application.Interfaces.Services.ICategoryServices
{
    public interface ICategoryCommandService 
    {
        Task<CategoryResponseDto> CreateCategoryAsync(CategoryRequestDto dto, Guid userId);
        Task<CategoryResponseDto> UpdateCategoryAsync(Guid CategoryId, UpdateCategoryRequestDto dto, Guid userId);
        Task DeleteCategoryAsync(Guid categoryId, Guid userId);  
        Task<ConfirmationResponseDto> SetCategoryEssentialAsync(UserCategorySettingsRequestDto dto, Guid categoryId, Guid userId);
    }
}
    