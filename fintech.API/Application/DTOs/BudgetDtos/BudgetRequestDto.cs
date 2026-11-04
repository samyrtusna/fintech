using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.BudgetDtos
{
    public class BudgetRequestDto
    {
        public Guid? CategoryId { get; set; }
        public BudgetType Type { get; set; }
        public Money Amount { get; set; } = null!;
        public Period Period { get; set; } = null!;
    }
}
