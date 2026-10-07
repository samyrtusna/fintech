using fintech.API.Application.DTOs.UserCurrenciesDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IUserCurrenciesService
    {
        Task<UserCurrenciesDto> AddUserCurrencyAsync(Guid userId, UserCurrenciesDto dto);
        Task<IEnumerable<UserCurrenciesDto>> GetUserCurrenciesAsync(Guid userId);
    }
}
