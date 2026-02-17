using AcademyIO.Core.Messages.Integration;
using AcademyIO.MessageBus;
using AcademyIO.Payments.API.Business;
using FluentValidation.Results;
using MediatR;

namespace AcademyIO.Payments.API.Services
{
    public class ValidatePaymentCourseCommandHandler : IRequestHandler<ValidatePaymentCourseCommand, ValidationResult>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMessageBus _bus;

        public ValidatePaymentCourseCommandHandler(IPaymentRepository paymentRepository, IMessageBus bus)
        {
            _paymentRepository = paymentRepository;
            _bus = bus;
        }

        public async Task<ValidationResult> Handle(ValidatePaymentCourseCommand message, CancellationToken cancellationToken)
        {
            if (!message.IsValid()) return message.ValidationResult;

            var payment = new Payment
            {
                CourseId = message.CourseId,
                StudentId = message.StudentId,
                CardName = message.CardName,
                CardNumber = message.CardNumber,
                CardExpirationDate = message.CardExpirationDate,
                CardCVV = message.CardCVV,
                Value = 0 // Valor simplificado para o fluxo
            };

            _paymentRepository.Add(payment);

            if (await _paymentRepository.UnitOfWork.Commit())
            {
                var eventMessage = new PaymentApprovedIntegrationEvent(message.CourseId, message.StudentId);
                await _bus.PublishAsync(eventMessage);
            }
            else
            {
                return new ValidationResult(new[] { new ValidationFailure("Payment", "Houve um erro ao persistir o pagamento") });
            }

            return new ValidationResult();
        }
    }
}