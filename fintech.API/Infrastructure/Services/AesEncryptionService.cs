using fintech.API.Application.Interfaces.Services;
using System.Security.Cryptography;
using System.Text;

namespace fintech.API.Infrastructure.Services
{
    public class AesEncryptionService : IEncryptionService
    {
        private readonly byte[] _key;

        public AesEncryptionService(IConfiguration configuration)
        {
            var key = configuration["Encryption:AesKey"];

            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException(
                    "Encryption:AesKey is not configured.");
            }

            try
            {
                _key = Convert.FromBase64String(key);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(
                    "Encryption:AesKey must be a valid Base64 string.");
            }

            if (_key.Length != 32)
            {
                throw new InvalidOperationException(
                    "Encryption:AesKey must be exactly 32 bytes.");
            }
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentException(
                    "Plain text cannot be empty.",
                    nameof(plainText));
            }

            byte[] nonce = RandomNumberGenerator.GetBytes(12);

            byte[] plainTextBytes =
                Encoding.UTF8.GetBytes(plainText);

            byte[] cipherText =
                new byte[plainTextBytes.Length];

            byte[] tag =
                new byte[16];

            using var aesGcm =
                new AesGcm(_key, tagSizeInBytes: 16);

            aesGcm.Encrypt(
                nonce,
                plainTextBytes,
                cipherText,
                tag);

            /*
             * Combined structure:
             *
             * [ nonce ][ tag ][ ciphertext ]
             *
             * nonce      = 12 bytes
             * tag        = 16 bytes
             * ciphertext = remaining bytes
             */

            byte[] result = new byte[
                nonce.Length +
                tag.Length +
                cipherText.Length];

            Buffer.BlockCopy(
                nonce,
                0,
                result,
                0,
                nonce.Length);

            Buffer.BlockCopy(
                tag,
                0,
                result,
                nonce.Length,
                tag.Length);

            Buffer.BlockCopy(
                cipherText,
                0,
                result,
                nonce.Length + tag.Length,
                cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentException(
                    "Cipher text cannot be empty.",
                    nameof(cipherText));
            }

            byte[] encryptedData;

            try
            {
                encryptedData =
                    Convert.FromBase64String(cipherText);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException(
                    "Encrypted value is not valid Base64.");
            }

            if (encryptedData.Length < 28)
            {
                throw new InvalidOperationException(
                    "Encrypted value is invalid.");
            }

            byte[] nonce =
                encryptedData[..12];

            byte[] tag =
                encryptedData[12..28];

            byte[] cipherTextBytes =
                encryptedData[28..];

            byte[] plainTextBytes =
                new byte[cipherTextBytes.Length];

            using var aesGcm =
                new AesGcm(_key, tagSizeInBytes: 16);

            aesGcm.Decrypt(
                nonce,
                cipherTextBytes,
                tag,
                plainTextBytes);

            return Encoding.UTF8.GetString(
                plainTextBytes);
        }
    }
}
