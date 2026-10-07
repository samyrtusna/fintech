using fintech.API.Application.DTOs.CategoryDtos;
using fintech.API.Application.DTOs.UserCurrenciesDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class UserCurrenciesMapper
    {
        public static UserCurrency MapToEntity(this UserCurrenciesDto dto)
        {
            return new UserCurrency
            {
                CurrencySymbol = dto.CurrencySymbol,
                CurrencyCode = dto.CurrencyCode,
                IsDefault = dto.IsDefault
            };
        }

        public static UserCurrenciesDto MapToDto(this UserCurrency userCurrency) 
        {
            return new UserCurrenciesDto
            {
                CurrencySymbol = userCurrency.CurrencySymbol,
                CurrencyCode = userCurrency.CurrencyCode,
                IsDefault = userCurrency.IsDefault
            };
        }
    }
}

        
