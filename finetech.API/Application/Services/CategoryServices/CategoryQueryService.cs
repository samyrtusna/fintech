using AutoMapper;
using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services.ICategoryServices;
using fintech.API.Application.Exceptions;

namespace fintech.API.Application.Services.CategoryServices
{
    public class CategoryQueryService(ICategoryRepository categoryRepository, IMapper mapper) : ICategoryQueryService
    {
        public async Task<IEnumerable<CategoryResponseDto>> GetAllCategoriesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var categories = await categoryRepository.GetByUserAsync(userId);

            return mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> GetCategoryByIdAsync(Guid categoryId, Guid userId)
        {
            if (categoryId == Guid.Empty)
            {
                throw new ArgumentException("Category ID cannot be empty.", nameof(categoryId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var category = await categoryRepository.GetAsync(categoryId) ?? throw new NotFoundException($"Category with ID '{categoryId}' not found.");

            if (!category.IsSystem && category.UserId != userId)
            {
                throw new UnauthorizedException("You do not have permission to view this category.");
            }

            return mapper.Map<CategoryResponseDto>(category);
        }
    }
}
