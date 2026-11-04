using fintech.API.Application.DTOs.UserDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<UserResponseDto> GetUserInformationsAsync(Guid userId); 
    }
}
