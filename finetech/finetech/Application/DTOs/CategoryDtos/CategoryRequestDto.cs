using fintech.Domain.Enums;

namespace fintech.Application.DTOs.CategoryDtos
{
    public class CategoryRequestDto
    {
        public string Name { get; set; } = null!;
        public CategoryType Type { get; set; } 
        public Guid? ParentCategoryId { get; set; }
    }
}
