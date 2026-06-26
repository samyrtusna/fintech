namespace fintech.API.Application.DTOs.AuthDtos
{
    public class LoginRequestDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
