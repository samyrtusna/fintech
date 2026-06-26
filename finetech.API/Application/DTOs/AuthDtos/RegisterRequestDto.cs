namespace fintech.API.Application.DTOs.AuthDtos
{
    public class RegisterRequestDto
    {
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}

