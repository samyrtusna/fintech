using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class CategoryMapper
    {
        public static Category MapToEntity(this CategoryRequestDto dto)
        {
            return new Category
            {
                Name = dto.Name,
                Type = dto.Type,
                ParentCategoryId = dto.ParentCategoryId
            };
        }

        public static CategoryResponseDto MapToResponseDto(this Category category)
        {
            return new CategoryResponseDto
            {
                Id = category.Id,
                Name = category.Name,
                Type = category.Type,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategory = category.ParentCategory?.Name
            };
        }

        public static void UpdateEntity(this UpdateCategoryRequestDto dto, Category category)
        {
            if (dto.Name != null)
            {
                category.Name = dto.Name;
            }
        } 
    }
}
