using AcademyIO.Payments.API.Business;
using AcademyIO.Payments.API.Security;
using Microsoft.Extensions.Options;

namespace AcademyIO.Payments.API.AntiCorruption;

public class PaymentCreditCardFacade(IPayPalGateway payPalGateway,
    IOptions<PaymentSettings> options,
    IEncryptionService encryptionService) : IPaymentCreditCardFacade
{
    private readonly PaymentSettings _settings = options.Value;

    public Transaction MakePayment(Payment payment)
    {
        var apiKey = _settings.ApiKey;
        var encriptionKey = _settings.EncriptionKey;

        // Decrypt card information for payment gateway
        var (cardNumber, expirationDate, cvv) = payment.GetDecryptedCardInformation(encryptionService);

        var serviceKey = payPalGateway.GetPayPalServiceKey(apiKey, encriptionKey);
        var cardHashKey = payPalGateway.GetCardHashKey(serviceKey, cardNumber);

        var transaction = payPalGateway.CommitTransaction(cardHashKey, payment.CourseId.ToString(), payment.Value);

        transaction.PaymentId = payment.Id;

        return transaction;
    }
}