using AcademyIO.Payments.API.Services;
using AcademyIO.WebAPI.Core.Controllers;
using AcademyIO.WebAPI.Core.User;
using FluentValidation.Results;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyIO.Payments.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : MainController
    {
        private readonly IMediator _mediator;
        private readonly IAspNetUser _aspNetUser;

        public PaymentsController(IMediator mediator, IAspNetUser aspNetUser)
        {
            _mediator = mediator;
            _aspNetUser = aspNetUser;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MakePayment(PaymentInputModel paymentModel)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var command = new ValidatePaymentCourseCommand(
                paymentModel.CourseId,
                _aspNetUser.GetUserId(),
                paymentModel.CardName,
                paymentModel.CardNumber,
                paymentModel.CardExpirationDate,
                paymentModel.CardCVV);

            var success = await _mediator.Send(command);

            return CustomResponse(command.ValidationResult ?? new ValidationResult());
        }
    }
}