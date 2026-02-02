using AcademyIO.Payments.API.Security;
using FluentAssertions;
using System;
using Xunit;

namespace AcademyIO.Payments.API.Tests.Security;

public class EncryptionServiceTests
{
    private const string ValidEncryptionKey = "this-is-a-very-secure-encryption-key-with-32-chars";
    private readonly IEncryptionService _encryptionService;

    public EncryptionServiceTests()
    {
        _encryptionService = new EncryptionService(ValidEncryptionKey);
    }

    [Fact]
    public void Constructor_WithKeyLessThan32Chars_ShouldThrowArgumentException()
    {
        // Arrange & Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => new EncryptionService("short-key"));
        ex.ParamName.Should().Be("encryptionKey");
    }

    [Fact]
    public void Encrypt_WithValidData_ShouldEncrypt()
    {
        // Arrange
        var plainText = "4532015112830366";

        // Act
        var encrypted = _encryptionService.Encrypt(plainText);

        // Assert
        encrypted.Should().NotBeNullOrEmpty();
        encrypted.Should().NotBe(plainText);
    }

    [Fact]
    public void Decrypt_WithEncryptedData_ShouldDecryptToOriginal()
    {
        // Arrange
        var plainText = "4532015112830366";
        var encrypted = _encryptionService.Encrypt(plainText);

        // Act
        var decrypted = _encryptionService.Decrypt(encrypted);

        // Assert
        decrypted.Should().Be(plainText);
    }

    [Fact]
    public void Encrypt_WithNullOrEmpty_ShouldReturnNullOrEmpty()
    {
        // Act
        var encryptedNull = _encryptionService.Encrypt(null!);
        var encryptedEmpty = _encryptionService.Encrypt("");

        // Assert
        encryptedNull.Should().BeNull();
        encryptedEmpty.Should().BeEmpty();
    }

    [Fact]
    public void Decrypt_WithNullOrEmpty_ShouldReturnNullOrEmpty()
    {
        // Act
        var decryptedNull = _encryptionService.Decrypt(null!);
        var decryptedEmpty = _encryptionService.Decrypt("");

        // Assert
        decryptedNull.Should().BeNull();
        decryptedEmpty.Should().BeEmpty();
    }

    [Fact]
    public void Encrypt_WithSameInput_ShouldProduceDifferentOutput()
    {
        // Arrange
        var plainText = "test data";

        // Act
        var encrypted1 = _encryptionService.Encrypt(plainText);
        var encrypted2 = _encryptionService.Encrypt(plainText);

        // Assert
        // Due to random IV, each encryption should produce different output
        encrypted1.Should().NotBe(encrypted2);
    }

    [Fact]
    public void Decrypt_WithInvalidBase64_ShouldThrowException()
    {
        // Arrange
        var invalidBase64 = "not-valid-base64!!!";

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => _encryptionService.Decrypt(invalidBase64));
        ex.Message.Should().Contain("Failed to decrypt data");
    }

    [Fact]
    public void Decrypt_WithWrongKey_ShouldThrowException()
    {
        // Arrange
        var plainText = "sensitive data";
        var encrypted = _encryptionService.Encrypt(plainText);
        var wrongKeyService = new EncryptionService("wrong-encryption-key-with-32-characters");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => wrongKeyService.Decrypt(encrypted));
        ex.Message.Should().Contain("Failed to decrypt data");
    }

    [Fact]
    public void EncryptDecrypt_WithMultipleDifferentValues_ShouldMaintainIntegrity()
    {
        // Arrange
        var testValues = new[]
        {
            "4532015112830366",
            "12/28",
            "123",
            "John Doe",
            "very-long-string-with-special-chars-!@#$%^&*()"
        };

        // Act & Assert
        foreach (var value in testValues)
        {
            var encrypted = _encryptionService.Encrypt(value);
            var decrypted = _encryptionService.Decrypt(encrypted);
            decrypted.Should().Be(value);
        }
    }
}
