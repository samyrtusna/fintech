using fintech.Application.DTOs.ApiResponsesDtos;
using fintech.Application.DTOs.AuthDtos;

namespace fintech.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterRequestDto dto);
        Task<string> LoginAsync(LoginRequestDto dto);
        Task<ConfirmationResponseDto> LogoutAsync();
    }
}
