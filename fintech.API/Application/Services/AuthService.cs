using fintech.API.Application.DTOs.ApiResponsesDtos;
using fintech.API.Application.DTOs.AuthDtos;
using fintech.API.Application.Interfaces.Repositories;
using fintech.API.Application.Interfaces.Services;
using fintech.API.Application.Exceptions;
using fintech.API.Application.Mappings;

namespace fintech.API.Application.Services
{
    public class AuthService(IUserRepository userRepository,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService,
        IPasswordHasher passwordHasher,
        ICookieService cookieService) : IAuthService
    {

        public async Task<string> RegisterAsync(RegisterRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto); 
            var user = await userRepository.GetByEmailAsync(dto.Email);
            if (user != null)
            {
                throw new DuplicateValueException($"A user with email {dto.Email} already exists.");
            }

            var newUser = dto.MapToEntity();
            newUser.PasswordHash = passwordHasher.HashPassword(dto.Password);

            await userRepository.AddAsync(newUser);
            await userRepository.SaveChangesAsync();
            

            var accessToken = jwtService.GenerateAccessToken(newUser);
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(newUser.Id);
            cookieService.SetTokenToCookie(refreshToken);

            return accessToken;
        }

        public async Task<string> LoginAsync(LoginRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await userRepository.GetByEmailAsync(dto.Email);
            var isPasswordValid = user != null && passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

            if (user == null || !isPasswordValid)
            {
                throw new BadRequestException("Invalid email or password.");
            }
            var accessToken = jwtService.GenerateAccessToken(user);
            var refreshToken = await refreshTokenService.CreateRefreshTokenAsync(user.Id);

            cookieService.SetTokenToCookie(refreshToken);
            return accessToken;
        }

        public async Task<ConfirmationResponseDto> LogoutAsync()
        {
            var token = cookieService.RetrieveTokenFromCookie();
            await refreshTokenService.RevokeRefreshTokenAsync(token);
            return new ConfirmationResponseDto { Message = " User Logged out successful." };
        }
    }
}
