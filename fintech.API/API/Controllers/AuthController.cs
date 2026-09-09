using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IRefreshTokenService refreshTokenService) : BaseController
    {

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponsesDto<string>>> RegisterAsync(RegisterRequestDto dto)
        {
            var response = await authService.RegisterAsync(dto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponsesDto<string>>> LoginAsync(LoginRequestDto dto)
        {
            var response = await authService.LoginAsync(dto);
            return Ok(response);
        }

        [Authorize (Policy = "UserPolicy")]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponsesDto<ConfirmationResponseDto>>> LogoutAsync()
        {
            var response = await authService.LogoutAsync();
            return Ok(response);
        }
         

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponsesDto<string>>> RefreshTokenAsync()
        {
            var response = await refreshTokenService.RefreshTokenAsync();
            return Ok(response);
        }
    }
}
