using fintech.API.Application.DTOs.UserDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class UserMapper
    {
        public static UserResponseDto MapToDto(this User user)
        {
            return new UserResponseDto
            {
                Email = user.Email,
                Username = user.Username,
                Role = user.Role,
                CreatedAt = user.CreatedAt,
                UserCurrencies = user.UserCurrencies.Select(uc => uc.MapToDto()).ToList()
            };
        }
    }
}
