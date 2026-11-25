using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.GoalDtos
{
    public class GoalResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public FinancialType? TargetType { get; set; }
        public Money TargetAmount { get; set; } = null!;
        public Period Period { get; set; } = null!;
        public decimal GoalProgress { get; set; } = 0;
        public Category? Category { get; set; }
    }
}
