using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.DTOs.UserCurrenciesDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.ValueObjects;
using System.Linq.Expressions;

namespace fintech.API.Application.Mappings
{
    public static class UserCurrenciesMapper
    {
        public static UserCurrency MapToEntity(this UserCurrenciesDto dto, Guid userId)
        {
            return new UserCurrency
            {
                UserId = userId,
                Currency = new Currency (dto.Currency.Code, dto.Currency.Symbol),
                IsDefault = dto.IsDefault
            };
        }

        public static UserCurrenciesDto MapToDto(this UserCurrency userCurrency) 
        {
            return new UserCurrenciesDto
            {
                Currency = new Currency (userCurrency.Currency.Code, userCurrency.Currency.Symbol),
                IsDefault = userCurrency.IsDefault
            };
        }
    }
}

        
