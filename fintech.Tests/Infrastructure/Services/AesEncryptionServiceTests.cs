using fintech.API.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;

namespace fintech.Tests.Infrastructure.Services
{
    public class AesEncryptionServiceTests
    {
        private static IConfiguration CreateConfiguration(string? key)
        {
            var settings = new Dictionary<string, string?>();

            if (key != null)
            {
                settings["Encryption:AesKey"] = key;
            }

            return new ConfigurationBuilder()
                .AddInMemoryCollection(settings)
                .Build();
        }

        private static string GenerateValidKey()
        {
            return Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(32));
        }

        [Fact]
        public void Constructor_WhenKeyIsMissing_ShouldThrowInvalidOperationException()
        {
            var configuration = CreateConfiguration(null);

            var exception = Assert.Throws<InvalidOperationException>(
                () => new AesEncryptionService(configuration));

            Assert.Equal(
                "Encryption:AesKey is not configured.",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenKeyIsEmpty_ShouldThrowInvalidOperationException()
        {
            var configuration = CreateConfiguration("");

            var exception = Assert.Throws<InvalidOperationException>(
                () => new AesEncryptionService(configuration));

            Assert.Equal(
                "Encryption:AesKey is not configured.",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenKeyIsInvalidBase64_ShouldThrowInvalidOperationException()
        {
            var configuration = CreateConfiguration("not-valid-base64!!!");

            var exception = Assert.Throws<InvalidOperationException>(
                () => new AesEncryptionService(configuration));

            Assert.Equal(
                "Encryption:AesKey must be a valid Base64 string.",
                exception.Message);
        }

        [Fact]
        public void Constructor_WhenKeyIsNot32Bytes_ShouldThrowInvalidOperationException()
        {
            var invalidKey = Convert.ToBase64String(
                RandomNumberGenerator.GetBytes(16));

            var configuration = CreateConfiguration(invalidKey);

            var exception = Assert.Throws<InvalidOperationException>(
                () => new AesEncryptionService(configuration));

            Assert.Equal(
                "Encryption:AesKey must be exactly 32 bytes.",
                exception.Message);
        }

        [Fact]
        public void Encrypt_WhenPlainTextIsEmpty_ShouldThrowArgumentException()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var exception = Assert.Throws<ArgumentException>(
                () => service.Encrypt(""));

            Assert.Equal("plainText", exception.ParamName);
            Assert.Equal(
                "Plain text cannot be empty. (Parameter 'plainText')",
                exception.Message);
        }

        [Fact]
        public void Encrypt_ShouldReturnBase64CipherText()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var encrypted = service.Encrypt("Hello World");

            Assert.False(string.IsNullOrWhiteSpace(encrypted));
            Assert.DoesNotContain("Hello World", encrypted);

            var encryptedBytes =
                Convert.FromBase64String(encrypted);

            Assert.True(encryptedBytes.Length >= 28);
        }

        [Fact]
        public void Encrypt_ShouldProduceDifferentCipherTextForSamePlainText()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var first = service.Encrypt("Hello World");
            var second = service.Encrypt("Hello World");

            Assert.NotEqual(first, second);
        }

        [Fact]
        public void Decrypt_WhenCipherTextIsEmpty_ShouldThrowArgumentException()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var exception = Assert.Throws<ArgumentException>(
                () => service.Decrypt(""));

            Assert.Equal("cipherText", exception.ParamName);
            Assert.Equal(
                "Cipher text cannot be empty. (Parameter 'cipherText')",
                exception.Message);
        }

        [Fact]
        public void Decrypt_WhenCipherTextIsInvalidBase64_ShouldThrowInvalidOperationException()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.Decrypt("not-valid-base64!!!"));

            Assert.Equal(
                "Encrypted value is not valid Base64.",
                exception.Message);
        }

        [Fact]
        public void Decrypt_WhenEncryptedDataIsTooShort_ShouldThrowInvalidOperationException()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var invalidCipherText =
                Convert.ToBase64String(new byte[27]);

            var exception = Assert.Throws<InvalidOperationException>(
                () => service.Decrypt(invalidCipherText));

            Assert.Equal(
                "Encrypted value is invalid.",
                exception.Message);
        }

        [Fact]
        public void Decrypt_ShouldReturnOriginalPlainText()
        {
            var configuration =
                CreateConfiguration(GenerateValidKey());

            var service =
                new AesEncryptionService(configuration);

            var plainText = "Sensitive financial information";

            var encrypted = service.Encrypt(plainText);
            var decrypted = service.Decrypt(encrypted);

            Assert.Equal(plainText, decrypted);
        }

        [Fact]
        public void Decrypt_WithDifferentKey_ShouldThrowAuthenticationTagMismatchException()
        {
            var encryptionKey = GenerateValidKey();
            var differentKey = GenerateValidKey();

            var encryptionConfiguration =
                CreateConfiguration(encryptionKey);

            var decryptionConfiguration =
                CreateConfiguration(differentKey);

            var encryptionService =
                new AesEncryptionService(encryptionConfiguration);

            var decryptionService =
                new AesEncryptionService(decryptionConfiguration);

            var encrypted =
                encryptionService.Encrypt("Sensitive data");

            Assert.Throws<AuthenticationTagMismatchException>(
                () => decryptionService.Decrypt(encrypted));
        }
    }
}