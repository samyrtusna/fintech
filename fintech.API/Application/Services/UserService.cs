using fintech.API.Application.DTOs.UserDtos;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Mappings;

namespace fintech.API.Application.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {

        public async Task<UserResponseDto> GetUserInformations (Guid userId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(userId));
            }
            var user = await userRepository.GetUserByIdAsync(userId) ?? throw new NotFoundException($"User with ID {userId} not found.");
            return user.MapToDto();
        }
    }
}
