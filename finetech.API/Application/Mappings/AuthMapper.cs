using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Domain.Entities;

namespace fintech.API.Application.Mappings
{
    public static class AuthMapper
    {
        public static User MapToEntity(this RegisterRequestDto dto)
        {
            return new User
            {
                Username = dto.Username, 
                Email = dto.Email,
                PasswordHash = "HashPassword",
            };
        }
    }
}
