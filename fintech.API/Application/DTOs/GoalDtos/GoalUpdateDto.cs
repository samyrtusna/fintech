using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.GoalDtos
{
    public class GoalUpdateDto
    {
        public string? Title { get; set; }
        public Money? TargetAmount { get; set; }
    }
}
