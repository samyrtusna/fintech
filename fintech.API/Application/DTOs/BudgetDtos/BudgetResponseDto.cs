using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.BudgetDtos
{
    public class BudgetResponseDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CategoryId { get; set; }
        public BudgetType Type { get; set; }
        public Money Amount { get; set; } = null!;
        public Period Period { get; set; } = null!;
        public Category? Category { get; set; }
    }
}
