using AcademyIO.Core.DomainObjects.DTOs;
using AcademyIO.Core.Messages.Notifications;
using AcademyIO.Payments.API.Security;
using MediatR;

namespace AcademyIO.Payments.API.Business;

public class PaymentService(IPaymentCreditCardFacade paymentCreditCardFacade,
                              IPaymentRepository paymentRepository,
                              IMediator mediator,
                              IEncryptionService encryptionService,
                              ICardValidationService cardValidationService) : IPaymentService
{
    public async Task<bool> MakePaymentCourse(PaymentCourse paymentCourse)
    {
        // Validate card information before processing
        var validationResult = cardValidationService.ValidateCard(
            paymentCourse.CardNumber,
            paymentCourse.CardExpirationDate,
            paymentCourse.CardCVV,
            paymentCourse.CardName);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                await mediator.Publish(new DomainNotification("Payment", error));
            return false;
        }

        var payment = new Payment
        {
            Value = paymentCourse.Total,
            CardName = paymentCourse.CardName,
            StudentId = paymentCourse.StudentId,
            CourseId = paymentCourse.CourseId
        };

        // Set encrypted card information
        payment.SetCardInformation(
            paymentCourse.CardNumber,
            paymentCourse.CardExpirationDate,
            paymentCourse.CardCVV,
            encryptionService);

        var transaction = paymentCreditCardFacade.MakePayment(payment);

        if (transaction.StatusTransaction == StatusTransaction.Accept)
        {
            paymentRepository.Add(payment);
            paymentRepository.AddTransaction(transaction);

            await paymentRepository.UnitOfWork.Commit();
            return true;
        }

        await mediator.Publish(new DomainNotification("Payment", "The transaction was declined"));
        return false;
    }

}