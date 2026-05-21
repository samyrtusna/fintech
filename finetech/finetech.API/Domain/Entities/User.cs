using fintech.API.Domain.Enums;

namespace fintech.API.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Email { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string BaseCurrency { get; set; } = "USD";
        public List<RefreshToken>? RefreshTokens { get; set; } = [];
    }
}
 