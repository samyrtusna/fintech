using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.AuthDtos;

namespace fintech.API.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<string> LoginAsync(LoginRequestDto dto);
        Task<ConfirmationResponseDto> LogoutAsync();
    }
}
