using fintech.API.Application.DTOs.FinancialTransactionDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.Mappings
{
    public static class FinancialTransactionMapper
    {
        public static FinancialTransaction MapToEntity(this FinancialTransactionRequestDto dto, Guid userId)
        {
            return new FinancialTransaction
            {
                UserId = userId,
                CategoryId = dto.CategoryId,
                Amount = new Money(dto.Amount.Amount, new Currency(dto.Amount.Currency!.Code, dto.Amount.Currency.Symbol)),
                Description = dto.Description??string.Empty,
                IsEssential = dto.IsEssential??false,
                TransactionDate = dto.TransactionDate??DateTime.UtcNow,
            };
        }

        public static FinancialTransactionResponseDto MapToResponseDto(this FinancialTransaction entity)
        {
            return new FinancialTransactionResponseDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                Type = entity.Type,
                Amount = entity.Amount,
                ExchangeRate = entity.ExchangeRate,
                BaseAmount = entity.BaseAmount,
                Description = entity.Description ?? string.Empty,
                IsEssential = entity.IsEssential,
                TransactionDate = DateOnly.FromDateTime(entity.TransactionDate),
                CategoryName = entity.Category?.Name ?? string.Empty
            };
        }

        public static void UpdateEntity(this UpdateFinancialTransactionRequestDto dto, FinancialTransaction entity)
        {
            if (dto.Description != null)
            {
                entity.Description = dto.Description;
            }

            if (dto.IsEssential.HasValue)
            {
                entity.IsEssential = dto.IsEssential.Value;
            }
        }
    }
}
