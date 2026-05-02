using fintech.Domain.Entities;
using fintech.Domain.Enums;

namespace fintech.Application.DTOs.CategoryDtos
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryType Type { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategory { get; set; }
    }
}
