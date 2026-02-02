namespace AcademyIO.Payments.API.Security;

/// <summary>
/// Service for validating credit card information according to industry standards.
/// Implements Luhn algorithm and expiration date validation.
/// </summary>
public interface ICardValidationService
{
    /// <summary>
    /// Validates card number using Luhn algorithm
    /// </summary>
    /// <param name="cardNumber">The card number to validate</param>
    /// <returns>True if valid, false otherwise</returns>
    bool ValidateCardNumber(string cardNumber);

    /// <summary>
    /// Validates card expiration date
    /// </summary>
    /// <param name="expirationDate">Date in MM/YY format</param>
    /// <returns>True if card is not expired, false otherwise</returns>
    bool ValidateExpirationDate(string expirationDate);

    /// <summary>
    /// Validates CVV format
    /// </summary>
    /// <param name="cvv">CVV code (3-4 digits)</param>
    /// <returns>True if valid format, false otherwise</returns>
    bool ValidateCVV(string cvv);

    /// <summary>
    /// Validates all card information at once
    /// </summary>
    /// <param name="cardNumber">Card number</param>
    /// <param name="expirationDate">Expiration date in MM/YY format</param>
    /// <param name="cvv">CVV code</param>
    /// <param name="cardName">Cardholder name</param>
    /// <returns>Validation result with error messages if invalid</returns>
    CardValidationResult ValidateCard(string cardNumber, string expirationDate, string cvv, string cardName);
}

public class CardValidationService : ICardValidationService
{
    public bool ValidateCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        // Remove spaces and hyphens
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        // Check if it's all digits
        if (!cardNumber.All(char.IsDigit))
            return false;

        // Check length (typically 13-19 digits)
        if (cardNumber.Length < 13 || cardNumber.Length > 19)
            return false;

        // Apply Luhn algorithm
        return LuhnCheck(cardNumber);
    }

    public bool ValidateExpirationDate(string expirationDate)
    {
        if (string.IsNullOrWhiteSpace(expirationDate))
            return false;

        // Expected format: MM/YY
        if (!expirationDate.Contains("/"))
            return false;

        var parts = expirationDate.Split('/');
        if (parts.Length != 2)
            return false;

        if (!int.TryParse(parts[0], out var month) || !int.TryParse(parts[1], out var year))
            return false;

        // Validate month
        if (month < 1 || month > 12)
            return false;

        // Convert two-digit year to four-digit year (assume 20xx)
        var currentYear = DateTime.UtcNow.Year;
        var currentMonth = DateTime.UtcNow.Month;
        var currentTwoDigitYear = currentYear % 100;

        int fullYear;
        if (year < currentTwoDigitYear)
        {
            // If year is less than current year's last two digits, assume next century
            fullYear = 2000 + year + 100;
        }
        else
        {
            // Otherwise, assume current century
            fullYear = 2000 + year;
        }

        // Create expiry date at the end of the month
        var expiryDate = new DateTime(fullYear, month, DateTime.DaysInMonth(fullYear, month), 23, 59, 59);

        return expiryDate > DateTime.UtcNow;
    }

    public bool ValidateCVV(string cvv)
    {
        if (string.IsNullOrWhiteSpace(cvv))
            return false;

        // CVV should be 3-4 digits
        return cvv.Length >= 3 && cvv.Length <= 4 && cvv.All(char.IsDigit);
    }

    public CardValidationResult ValidateCard(string cardNumber, string expirationDate, string cvv, string cardName)
    {
        var result = new CardValidationResult();

        if (string.IsNullOrWhiteSpace(cardName))
            result.Errors.Add("Cardholder name is required");

        if (!ValidateCardNumber(cardNumber))
            result.Errors.Add("Card number is invalid");

        if (!ValidateExpirationDate(expirationDate))
            result.Errors.Add("Card expiration date is invalid or expired");

        if (!ValidateCVV(cvv))
            result.Errors.Add("Card CVV is invalid");

        return result;
    }

    /// <summary>
    /// Implements the Luhn algorithm for credit card validation
    /// </summary>
    private bool LuhnCheck(string cardNumber)
    {
        int sum = 0;
        bool isSecondDigit = false;

        for (int i = cardNumber.Length - 1; i >= 0; i--)
        {
            int digit = cardNumber[i] - '0';

            if (isSecondDigit)
            {
                digit *= 2;
                if (digit > 9)
                    digit -= 9;
            }

            sum += digit;
            isSecondDigit = !isSecondDigit;
        }

        return sum % 10 == 0;
    }
}

/// <summary>
/// Result of card validation
/// </summary>
public class CardValidationResult
{
    public List<string> Errors { get; set; } = new();

    public bool IsValid => Errors.Count == 0;
}
