using AcademyIO.Core.Messages.Integration;
using AcademyIO.MessageBus;
using FluentValidation.Results;
using MediatR;

namespace AcademyIO.Payments.API.Services
{
    public class PaymentRequestedIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public PaymentRequestedIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void SetResponder()
        {
            _bus.RespondAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(async request =>
                await RegisterPayment(request));

            _bus.AdvancedBus.Connected += OnConnect;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetResponder();
            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            SetResponder();
        }

        private async Task<ResponseMessage> RegisterPayment(PaymentRegisteredIntegrationEvent message)
        {
            var command = new ValidatePaymentCourseCommand(message.CourseId, message.StudentId, message.CardName,
                                                        message.CardNumber, message.CardExpirationDate,
                                                        message.CardCVV);
            ValidationResult validationResult;
            bool success;

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                success = await mediator.Send(command);
                validationResult = command.ValidationResult ?? new ValidationResult();
            }

            if (!success && validationResult.IsValid)
                validationResult.Errors.Add(new ValidationFailure(string.Empty, "Falha ao realizar pagamento."));

            return new ResponseMessage(validationResult);
        }
    }
}
