using System.Security.Cryptography;
using System.Text;

namespace AcademyIO.Payments.API.Security;

/// <summary>
/// Service for encrypting and decrypting sensitive payment information (PCI-DSS compliance).
/// Uses AES encryption with a configurable key.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts sensitive data
    /// </summary>
    /// <param name="plainText">The plain text to encrypt</param>
    /// <returns>Base64 encoded encrypted data</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts encrypted data
    /// </summary>
    /// <param name="encryptedText">Base64 encoded encrypted data</param>
    /// <returns>The decrypted plain text</returns>
    string Decrypt(string encryptedText);
}

public class EncryptionService : IEncryptionService
{
    private readonly string _encryptionKey;

    public EncryptionService(string encryptionKey)
    {
        if (string.IsNullOrWhiteSpace(encryptionKey) || encryptionKey.Length < 32)
            throw new ArgumentException("Encryption key must be at least 32 characters long", nameof(encryptionKey));

        _encryptionKey = encryptionKey;
    }

    public string Encrypt(string plainText)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return plainText;

        using (var aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 32));
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            var iv = aes.IV;

            using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
            using (var ms = new MemoryStream())
            {
                // Write IV to the beginning of the stream
                ms.Write(iv, 0, iv.Length);

                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }

    public string Decrypt(string encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
            return encryptedText;

        try
        {
            var buffer = Convert.FromBase64String(encryptedText);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.Substring(0, 32));
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                // Extract IV from the beginning of the buffer
                var iv = new byte[aes.IV.Length];
                Array.Copy(buffer, 0, iv, 0, iv.Length);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                using (var ms = new MemoryStream(buffer, iv.Length, buffer.Length - iv.Length))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to decrypt data. The encryption key may be incorrect.", ex);
        }
    }
}
