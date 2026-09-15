using fintech.API.Infrastructure.Services;

namespace fintech.Tests.Infrastructure.Services
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_ShouldReturnNonEmptyHash()
        {
            var password = "MySecurePassword123";

            var hash = _passwordHasher.HashPassword(password);

            Assert.False(string.IsNullOrWhiteSpace(hash));
            Assert.NotEqual(password, hash);
        }

        [Fact]
        public void HashPassword_ShouldProduceDifferentHashesForSamePassword()
        {
            var password = "MySecurePassword123";

            var firstHash = _passwordHasher.HashPassword(password);
            var secondHash = _passwordHasher.HashPassword(password);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
        {
            var password = "MySecurePassword123";

            var hash = _passwordHasher.HashPassword(password);

            var result = _passwordHasher.VerifyPassword(password, hash);

            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
        {
            var password = "MySecurePassword123";
            var wrongPassword = "WrongPassword123";

            var hash = _passwordHasher.HashPassword(password);

            var result = _passwordHasher.VerifyPassword(wrongPassword, hash);

            Assert.False(result);
        }

        [Fact]
        public void VerifyPassword_WithInvalidHash_ShouldThrowSaltParseException()
        {
            var password = "MySecurePassword123";
            var invalidHash = "not-a-valid-bcrypt-hash";

            Assert.Throws<BCrypt.Net.SaltParseException>(
                () => _passwordHasher.VerifyPassword(password, invalidHash));
        }
    }
}