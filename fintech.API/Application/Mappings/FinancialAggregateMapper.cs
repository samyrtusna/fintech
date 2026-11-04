using fintech.API.Application.DTOs.FinancialAggregatesDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Mappings
{
    public static class FinancialAggregateMapper
    {
        public static FinancialAggregate MapToEntity(this FinancialAggregateDto dto, Guid userId)
        {
            return new FinancialAggregate
            {
                UserId = userId,
                Period = new Period(dto.Period.StartDate, dto.Period.EndDate),
                PeriodType = dto.PeriodType,
                Type = dto.Type,
                Name = dto.Name,
                Amount = new Money(dto.Amount.Amount, dto.Amount.Currency != null ? new Currency(dto.Amount.Currency.Code, dto.Amount.Currency.Symbol) : null),
                Variation = dto.Variation,
            };
        }

        public static FinancialAggregateDto MapToDto(this FinancialAggregate entity)
        {
            return new FinancialAggregateDto
            {
                Period = entity.Period,
                PeriodType = entity.PeriodType,
                Type = entity.Type,
                Name = entity.Name,
                Amount = entity.Amount,
                Variation = entity.Variation,
            };
        }
    }
}
