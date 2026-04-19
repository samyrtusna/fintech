using AutoMapper;
using fintech.Application.DTOs.ApiResponsesDtos;
using fintech.Application.DTOs.AuthDtos;
using fintech.Application.Exceptions;
using fintech.Application.Interfaces.Repositories;
using fintech.Application.Interfaces.Services;
using fintech.Domain.Entities;

namespace fintech.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ICookieService _cookieService;


        public AuthService(IUserRepository userRepository,
            IJwtService jwtService, 
            IRefreshTokenService refreshTokenService, 
            IMapper mapper, 
            IPasswordHasher passwordHasher, 
            ICookieService cookieService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _refreshTokenService = refreshTokenService;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _cookieService = cookieService;
        }

        public async Task<string> RegisterAsync(RegisterRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user != null)
            {
                throw new DuplicateValueException($"A user with email {dto.Email} already exists.");
            }

            var newUser = _mapper.Map<User>(dto);
            newUser.PasswordHash = _passwordHasher.HashPassword(dto.Password);

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            

            var accessToken = _jwtService.GenerateAccessToken(newUser);
            var refreshToken = await _refreshTokenService.AddNewRefreshToken(newUser.Id);

            _cookieService.SetTokenToCookie(refreshToken);

            return accessToken;
        }

        public async Task<string> LoginAsync(LoginRequestDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            var user = await _userRepository.GetByEmailAsync(dto.Email);
            var isPasswordValid = user != null && _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);

            if (user == null || !isPasswordValid)
            {
                throw new BadRequestException("Invalid email or password.");
            }
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = await _refreshTokenService.AddNewRefreshToken(user.Id);

            _cookieService.SetTokenToCookie(refreshToken);
            return accessToken;
        }

        public async Task<ConfirmationResponseDto> LogoutAsync()
        {
            var token = _cookieService.RetrieveTokenFromCookie();
            await _refreshTokenService.RevokeToken(token);
            return new ConfirmationResponseDto { Message = " User Logged out successful." };
        }
    }
}
