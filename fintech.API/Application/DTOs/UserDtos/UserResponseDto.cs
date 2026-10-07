using fintech.API.Application.DTOs.UserCurrenciesDtos;
using fintech.API.Domain.Entities;
using fintech.API.Domain.Enums;

namespace fintech.API.Application.DTOs.UserDtos
{
    public class UserResponseDto 
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<UserCurrenciesDto> UserCurrencies { get; set; } = [];
    }
}
