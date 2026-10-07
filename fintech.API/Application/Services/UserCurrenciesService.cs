using fintech.API.Application.DTOs.UserCurrenciesDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;

namespace fintech.API.Application.Services
{
    public class UserCurrenciesService(IUserCurrenciesRepositoy userCurrenciesRepositoy) : IUserCurrenciesService
    {
        public async Task<UserCurrenciesDto> AddUserCurrencyAsync(Guid userId, UserCurrenciesDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var existingUserCurrencies = await userCurrenciesRepositoy.GetUserCurrenciesAsync(userId);
            if(existingUserCurrencies.Count() == 4)
            {
                throw new BadRequestException("A user can have at most 4 currencies.");
            }
            if(dto.IsDefault && existingUserCurrencies.Any(uc => uc.IsDefault))
            {
                throw new BadRequestException("A user can have only one default currency.");
            }
            if (existingUserCurrencies.Any(uc => uc.CurrencyCode.Equals(dto.CurrencyCode, StringComparison.CurrentCultureIgnoreCase)))
            {
                throw new BadRequestException($"The currency with code '{dto.CurrencyCode}' already exists for this user.");
            }
            var newUserCurrency = dto.MapToEntity(); 
            newUserCurrency.UserId = userId;
            await userCurrenciesRepositoy.AddAsync(newUserCurrency);
            await userCurrenciesRepositoy.SaveChangesAsync();
            return newUserCurrency.MapToDto();
        }

        public async Task<IEnumerable<UserCurrenciesDto>> GetUserCurrenciesAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }

            var userCurrencies = await userCurrenciesRepositoy.GetUserCurrenciesAsync(userId);

            return userCurrencies.Select(uc => uc.MapToDto());
        }
    }
}
