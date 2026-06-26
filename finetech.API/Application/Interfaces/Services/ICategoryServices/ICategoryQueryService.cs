using fintech.API.Application.DTOs.CategoryDtos;

namespace fintech.API.Application.Interfaces.Services.ICategoryServices
{
    public interface ICategoryQueryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(Guid userId);
        Task<CategoryResponseDto> GetCategoryByIdAsync(Guid categoryId, Guid userId);
    }
}
