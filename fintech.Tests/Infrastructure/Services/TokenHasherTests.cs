using fintech.API.Infrastructure.Services;
using System.Security.Cryptography;
using System.Text;

namespace fintech.Tests.Infrastructure.Services
{
    public class TokenHasherTests
    {
        private readonly TokenHasher _tokenHasher;

        public TokenHasherTests()
        {
            _tokenHasher = new TokenHasher();
        }

        [Fact]
        public void HashToken_ShouldReturnNonEmptyHash()
        {
            var token = "my-refresh-token";

            var hash = _tokenHasher.HashToken(token);

            Assert.False(string.IsNullOrWhiteSpace(hash));
        }

        [Fact]
        public void HashToken_SameToken_ShouldReturnSameHash()
        {
            var token = "my-refresh-token";

            var firstHash = _tokenHasher.HashToken(token);
            var secondHash = _tokenHasher.HashToken(token);

            Assert.Equal(firstHash, secondHash);
        }

        [Fact]
        public void HashToken_DifferentTokens_ShouldReturnDifferentHashes()
        {
            var firstToken = "refresh-token-1";
            var secondToken = "refresh-token-2";

            var firstHash = _tokenHasher.HashToken(firstToken);
            var secondHash = _tokenHasher.HashToken(secondToken);

            Assert.NotEqual(firstHash, secondHash);
        }

        [Fact]
        public void HashToken_ShouldReturnValidBase64()
        {
            var token = "my-refresh-token";

            var hash = _tokenHasher.HashToken(token);

            var decodedHash = Convert.FromBase64String(hash);

            Assert.Equal(32, decodedHash.Length);
        }

        [Fact]
        public void HashToken_ShouldMatchExpectedSha256Hash()
        {
            var token = "my-refresh-token";

            using var sha256 = SHA256.Create();

            var expectedHash = Convert.ToBase64String(
                sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(token)));

            var actualHash = _tokenHasher.HashToken(token);

            Assert.Equal(expectedHash, actualHash);
        }

        [Fact]
        public void HashToken_EmptyToken_ShouldReturnValidSha256Hash()
        {
            var token = "";

            var hash = _tokenHasher.HashToken(token);

            var decodedHash = Convert.FromBase64String(hash);

            Assert.Equal(32, decodedHash.Length);
        }
    }
}