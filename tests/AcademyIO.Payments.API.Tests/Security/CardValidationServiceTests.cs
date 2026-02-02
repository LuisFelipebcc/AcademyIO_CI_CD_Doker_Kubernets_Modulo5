using AcademyIO.Payments.API.Security;
using FluentAssertions;
using System;
using Xunit;

namespace AcademyIO.Payments.API.Tests.Security;

public class CardValidationServiceTests
{
    private readonly ICardValidationService _cardValidationService;

    public CardValidationServiceTests()
    {
        _cardValidationService = new CardValidationService();
    }

    #region Card Number Validation Tests

    [Fact]
    public void ValidateCardNumber_WithValidCardNumber_ShouldReturnTrue()
    {
        // Arrange
        var validCardNumber = "4532015112830366"; // Valid Visa test number

        // Act
        var result = _cardValidationService.ValidateCardNumber(validCardNumber);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCardNumber_WithInvalidChecksum_ShouldReturnFalse()
    {
        // Arrange
        var invalidCardNumber = "4532015112830367"; // Invalid checksum

        // Act
        var result = _cardValidationService.ValidateCardNumber(invalidCardNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCardNumber_WithLetters_ShouldReturnFalse()
    {
        // Arrange
        var invalidCardNumber = "453201511283036A";

        // Act
        var result = _cardValidationService.ValidateCardNumber(invalidCardNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCardNumber_WithTooShort_ShouldReturnFalse()
    {
        // Arrange
        var tooShortCardNumber = "12345";

        // Act
        var result = _cardValidationService.ValidateCardNumber(tooShortCardNumber);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCardNumber_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = _cardValidationService.ValidateCardNumber(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCardNumber_WithSpaces_ShouldRemoveAndValidate()
    {
        // Arrange
        var cardNumberWithSpaces = "4532 0151 1283 0366";

        // Act
        var result = _cardValidationService.ValidateCardNumber(cardNumberWithSpaces);

        // Assert
        result.Should().BeTrue();
    }

    #endregion

    #region Expiration Date Validation Tests

    [Fact]
    public void ValidateExpirationDate_WithValidFutureDate_ShouldReturnTrue()
    {
        // Arrange
        var nextYear = (DateTime.UtcNow.Year + 1) % 100;
        var validExpirationDate = $"12/{nextYear:D2}";

        // Act
        var result = _cardValidationService.ValidateExpirationDate(validExpirationDate);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateExpirationDate_WithExpiredDate_ShouldReturnFalse()
    {
        // Arrange
        var expiredExpirationDate = "01/20";

        // Act
        var result = _cardValidationService.ValidateExpirationDate(expiredExpirationDate);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateExpirationDate_WithInvalidMonth_ShouldReturnFalse()
    {
        // Arrange
        var invalidMonth = "13/25";

        // Act
        var result = _cardValidationService.ValidateExpirationDate(invalidMonth);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateExpirationDate_WithNull_ShouldReturnFalse()
    {
        // Act
        var result = _cardValidationService.ValidateExpirationDate(null!);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateExpirationDate_WithoutSlash_ShouldReturnFalse()
    {
        // Arrange
        var invalidFormat = "1225";

        // Act
        var result = _cardValidationService.ValidateExpirationDate(invalidFormat);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region CVV Validation Tests

    [Fact]
    public void ValidateCVV_With3Digits_ShouldReturnTrue()
    {
        // Arrange
        var validCVV = "123";

        // Act
        var result = _cardValidationService.ValidateCVV(validCVV);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCVV_With4Digits_ShouldReturnTrue()
    {
        // Arrange
        var validCVV = "1234";

        // Act
        var result = _cardValidationService.ValidateCVV(validCVV);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ValidateCVV_With2Digits_ShouldReturnFalse()
    {
        // Arrange
        var shortCVV = "12";

        // Act
        var result = _cardValidationService.ValidateCVV(shortCVV);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCVV_With5Digits_ShouldReturnFalse()
    {
        // Arrange
        var longCVV = "12345";

        // Act
        var result = _cardValidationService.ValidateCVV(longCVV);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ValidateCVV_WithLetters_ShouldReturnFalse()
    {
        // Arrange
        var invalidCVV = "12A";

        // Act
        var result = _cardValidationService.ValidateCVV(invalidCVV);

        // Assert
        result.Should().BeFalse();
    }

    #endregion

    #region Complete Card Validation Tests

    [Fact]
    public void ValidateCard_WithAllValidData_ShouldReturnValid()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var nextYear = (DateTime.UtcNow.Year + 1) % 100;
        var expirationDate = $"12/{nextYear:D2}";
        var cvv = "123";
        var cardName = "John Doe";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact]
    public void ValidateCard_WithInvalidCardNumber_ShouldReturnInvalid()
    {
        // Arrange
        var cardNumber = "1234567890123456";
        var nextYear = (DateTime.UtcNow.Year + 1) % 100;
        var expirationDate = $"12/{nextYear:D2}";
        var cvv = "123";
        var cardName = "John Doe";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Card number is invalid");
    }

    [Fact]
    public void ValidateCard_WithExpiredDate_ShouldReturnInvalid()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var expirationDate = "01/20";
        var cvv = "123";
        var cardName = "John Doe";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Card expiration date is invalid or expired");
    }

    [Fact]
    public void ValidateCard_WithInvalidCVV_ShouldReturnInvalid()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var nextYear = (DateTime.UtcNow.Year + 1) % 100;
        var expirationDate = $"12/{nextYear:D2}";
        var cvv = "12"; // Too short
        var cardName = "John Doe";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Card CVV is invalid");
    }

    [Fact]
    public void ValidateCard_WithEmptyCardName_ShouldReturnInvalid()
    {
        // Arrange
        var cardNumber = "4532015112830366";
        var futureYear = DateTime.UtcNow.Year + 1;
        var expirationDate = $"12/{futureYear:yy}";
        var cvv = "123";
        var cardName = "";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Cardholder name is required");
    }

    [Fact]
    public void ValidateCard_WithMultipleInvalidFields_ShouldReturnAllErrors()
    {
        // Arrange
        var cardNumber = "1234567890123456";
        var expirationDate = "01/20";
        var cvv = "12";
        var cardName = "";

        // Act
        var result = _cardValidationService.ValidateCard(cardNumber, expirationDate, cvv, cardName);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(4);
        result.Errors.Should().Contain("Cardholder name is required");
        result.Errors.Should().Contain("Card number is invalid");
        result.Errors.Should().Contain("Card expiration date is invalid or expired");
        result.Errors.Should().Contain("Card CVV is invalid");
    }

    #endregion
}
