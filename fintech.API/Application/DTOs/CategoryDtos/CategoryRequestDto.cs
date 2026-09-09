using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.CategoryDtos
{
    public class CategoryRequestDto
    {
        public string Name { get; set; } = null!;
        public FinancialType Type { get; set; } 
        public Guid? ParentCategoryId { get; set; }
    }
}
 