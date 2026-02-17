using AcademyIO.Core.Messages;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace AcademyIO.Payments.API.Services
{
    public class ValidatePaymentCourseCommand : Command, IRequest<ValidationResult>
    {
        public Guid CourseId { get; private set; }
        public Guid StudentId { get; private set; }
        public string CardName { get; private set; }
        public string CardNumber { get; private set; }
        public string CardExpirationDate { get; private set; }
        public string CardCVV { get; private set; }

        public ValidatePaymentCourseCommand(Guid courseId, Guid studentId, string cardName, string cardNumber, string cardExpirationDate, string cardCVV)
        {
            CourseId = courseId;
            StudentId = studentId;
            CardName = cardName;
            CardNumber = cardNumber;
            CardExpirationDate = cardExpirationDate;
            CardCVV = cardCVV;
        }

        public override bool IsValid()
        {
            ValidationResult = new ValidatePaymentCourseCommandValidation().Validate(this);
            return ValidationResult.IsValid;
        }
    }

    public class ValidatePaymentCourseCommandValidation : AbstractValidator<ValidatePaymentCourseCommand>
    {
        public ValidatePaymentCourseCommandValidation()
        {
            RuleFor(c => c.CardName)
                .NotEmpty()
                .WithMessage("Nome do cartão não informado");

            RuleFor(c => c.CardNumber)
                .CreditCard()
                .WithMessage("Número de cartão de crédito inválido");

            RuleFor(c => c.CardExpirationDate)
                .NotEmpty()
                .WithMessage("Data de expiração não informada");

            RuleFor(c => c.CardCVV)
                .Length(3, 4)
                .WithMessage("O CVV deve ter 3 ou 4 dígitos");
        }
    }
}