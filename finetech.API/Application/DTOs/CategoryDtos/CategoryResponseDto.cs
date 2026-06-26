using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.CategoryDtos
{
    public class CategoryResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public FinancialType Type { get; set; }
        public Guid? ParentCategoryId { get; set; }
        public string? ParentCategory { get; set; }
    }
}
