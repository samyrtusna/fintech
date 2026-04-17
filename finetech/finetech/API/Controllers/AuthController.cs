using fintech.Application.DTOs.ApiResponsesDtos;
using fintech.Application.DTOs.AuthDtos;
using fintech.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace fintech.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IRefreshTokenService _refreshTokenService;

        public AuthController(IAuthService authService, IRefreshTokenService refreshTokenService)
        {
            _authService = authService;
            _refreshTokenService = refreshTokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponsesDto<string>>> Register(RegisterRequestDto dto)
        {
            var response = await _authService.RegisterAsync(dto);
            return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponsesDto<string>>> Login(LoginRequestDto dto)
        {
            var response = await _authService.LoginAsync(dto);
            return Ok(response);
        }

        [Authorize (Policy = "UserPolicy")]
        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponsesDto<ConfirmationResponseDto>>> Logout()
        {
            var response = await _authService.LogoutAsync();
            return Ok(response);
        }

        [Authorize (Policy = "UserPolicy")]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponsesDto<string>>> RefreshToken()
        {
            var response = await _refreshTokenService.RefreshAsync();
            return Ok(response);
        }
    }
}
