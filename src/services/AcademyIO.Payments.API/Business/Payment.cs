using AcademyIO.Core.DomainObjects;
using AcademyIO.Payments.API.Security;

namespace AcademyIO.Payments.API.Business;

/// <summary>
/// Payment aggregate root for managing course payment transactions.
/// Card data is stored encrypted to ensure PCI-DSS compliance.
/// </summary>
public class Payment : Entity, IAggregateRoot
{
    public Guid CourseId { get; set; }
    public Guid StudentId { get; set; }
    public double Value { get; set; }

    /// <summary>
    /// Cardholder name
    /// </summary>
    public string CardName { get; set; }

    /// <summary>
    /// Encrypted card number (never stored in plaintext)
    /// </summary>
    public string EncryptedCardNumber { get; set; }

    /// <summary>
    /// Encrypted card expiration date in MM/YY format (never stored in plaintext)
    /// </summary>
    public string EncryptedCardExpirationDate { get; set; }

    /// <summary>
    /// Encrypted card CVV (never stored in plaintext)
    /// </summary>
    public string EncryptedCardCVV { get; set; }

    /// <summary>
    /// Last 4 digits of card (used for display/identification purposes)
    /// </summary>
    public string CardNumberLast4 { get; set; }

    public Transaction Transaction { get; set; }

    /// <summary>
    /// Sets card information with automatic encryption of sensitive data
    /// </summary>
    /// <param name="cardNumber">Plain text card number (will be encrypted)</param>
    /// <param name="expirationDate">Card expiration date in MM/YY format (will be encrypted)</param>
    /// <param name="cvv">Card CVV (will be encrypted)</param>
    /// <param name="encryptionService">Service to encrypt sensitive data</param>
    public void SetCardInformation(string cardNumber, string expirationDate, string cvv, IEncryptionService encryptionService)
    {
        if (encryptionService == null)
            throw new ArgumentNullException(nameof(encryptionService));

        // Encrypt sensitive data
        EncryptedCardNumber = encryptionService.Encrypt(cardNumber);
        EncryptedCardExpirationDate = encryptionService.Encrypt(expirationDate);
        EncryptedCardCVV = encryptionService.Encrypt(cvv);

        // Store last 4 digits for reference
        CardNumberLast4 = cardNumber.Length >= 4
            ? cardNumber.Substring(cardNumber.Length - 4)
            : cardNumber;
    }

    /// <summary>
    /// Decrypts card information (use with caution and only when necessary)
    /// </summary>
    /// <param name="encryptionService">Service to decrypt sensitive data</param>
    /// <returns>Decrypted card data (card number, expiration, CVV)</returns>
    public (string CardNumber, string ExpirationDate, string CVV) GetDecryptedCardInformation(IEncryptionService encryptionService)
    {
        if (encryptionService == null)
            throw new ArgumentNullException(nameof(encryptionService));

        return (
            encryptionService.Decrypt(EncryptedCardNumber),
            encryptionService.Decrypt(EncryptedCardExpirationDate),
            encryptionService.Decrypt(EncryptedCardCVV)
        );
    }
}