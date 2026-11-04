using fintech.API.Domain.ValueObjects;

namespace fintech.API.Application.DTOs.UserCurrenciesDtos
{
    public class UserCurrenciesDto
    {
        public Currency Currency { get; set; } = null!;
        public bool IsDefault { get; set; } = false;
    }
}
