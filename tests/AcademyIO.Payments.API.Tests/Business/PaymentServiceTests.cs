using AcademyIO.Core.Data;
using AcademyIO.Core.DomainObjects.DTOs;
using AcademyIO.Core.Messages.Notifications;
using AcademyIO.Payments.API.Business;
using AcademyIO.Payments.API.Security;
using FluentAssertions;
using MediatR;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Payments.API.Tests.Business;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentCreditCardFacade> _paymentCreditCardFacadeMock;
    private readonly Mock<IPaymentRepository> _paymentRepositoryMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IEncryptionService> _encryptionServiceMock;
    private readonly Mock<ICardValidationService> _cardValidationServiceMock;
    private readonly IPaymentService _paymentService;

    public PaymentServiceTests()
    {
        _paymentCreditCardFacadeMock = new Mock<IPaymentCreditCardFacade>();
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _mediatorMock = new Mock<IMediator>();
        _encryptionServiceMock = new Mock<IEncryptionService>();
        _cardValidationServiceMock = new Mock<ICardValidationService>();

        _paymentService = new PaymentService(
            _paymentCreditCardFacadeMock.Object,
            _paymentRepositoryMock.Object,
            _mediatorMock.Object,
            _encryptionServiceMock.Object,
            _cardValidationServiceMock.Object);
    }

    [Fact]
    public async Task MakePaymentCourse_WithValidCard_ShouldSucceed()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 500,
            CardName = "John Doe",
            CardNumber = "4532015112830366",
            CardExpirationDate = "12/28",
            CardCVV = "123"
        };

        var validationResult = new CardValidationResult();
        _cardValidationServiceMock
            .Setup(x => x.ValidateCard(
                paymentCourse.CardNumber,
                paymentCourse.CardExpirationDate,
                paymentCourse.CardCVV,
                paymentCourse.CardName))
            .Returns(validationResult);

        _encryptionServiceMock
            .Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns("encrypted_value");

        var transaction = new Transaction { StatusTransaction = StatusTransaction.Accept, PaymentId = Guid.NewGuid() };
        _paymentCreditCardFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(transaction);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _paymentRepositoryMock
            .Setup(x => x.UnitOfWork)
            .Returns(unitOfWorkMock.Object);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeTrue();
        _paymentRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Once);
        _paymentRepositoryMock.Verify(x => x.AddTransaction(It.IsAny<Transaction>()), Times.Once);
        unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
    }

    [Fact]
    public async Task MakePaymentCourse_WithInvalidCard_ShouldFail()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 500,
            CardName = "John Doe",
            CardNumber = "invalid",
            CardExpirationDate = "01/20",
            CardCVV = "12"
        };

        var validationResult = new CardValidationResult
        {
            Errors = new List<string>
            {
                "Card number is invalid",
                "Card expiration date is invalid or expired",
                "Card CVV is invalid"
            }
        };

        _cardValidationServiceMock
            .Setup(x => x.ValidateCard(
                paymentCourse.CardNumber,
                paymentCourse.CardExpirationDate,
                paymentCourse.CardCVV,
                paymentCourse.CardName))
            .Returns(validationResult);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeFalse();
        _paymentRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Never);
        _mediatorMock.Verify(x => x.Publish(It.IsAny<DomainNotification>(), It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task MakePaymentCourse_WithDeclinedTransaction_ShouldFail()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 500,
            CardName = "John Doe",
            CardNumber = "4532015112830366",
            CardExpirationDate = "12/28",
            CardCVV = "123"
        };

        var validationResult = new CardValidationResult();
        _cardValidationServiceMock
            .Setup(x => x.ValidateCard(
                paymentCourse.CardNumber,
                paymentCourse.CardExpirationDate,
                paymentCourse.CardCVV,
                paymentCourse.CardName))
            .Returns(validationResult);

        _encryptionServiceMock
            .Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns("encrypted_value");

        var transaction = new Transaction { StatusTransaction = StatusTransaction.Declined, PaymentId = Guid.NewGuid() };
        _paymentCreditCardFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(transaction);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeFalse();
        _paymentRepositoryMock.Verify(x => x.Add(It.IsAny<Payment>()), Times.Never);
        _mediatorMock.Verify(
            x => x.Publish(
                It.Is<DomainNotification>(n => n.Value.Contains("declined")),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task MakePaymentCourse_ShouldEncryptCardData()
    {
        // Arrange
        var paymentCourse = new PaymentCourse
        {
            CourseId = Guid.NewGuid(),
            StudentId = Guid.NewGuid(),
            Total = 500,
            CardName = "John Doe",
            CardNumber = "4532015112830366",
            CardExpirationDate = "12/28",
            CardCVV = "123"
        };

        var validationResult = new CardValidationResult();
        _cardValidationServiceMock
            .Setup(x => x.ValidateCard(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(validationResult);

        _encryptionServiceMock
            .Setup(x => x.Encrypt(It.IsAny<string>()))
            .Returns("encrypted_value");

        var transaction = new Transaction { StatusTransaction = StatusTransaction.Accept, PaymentId = Guid.NewGuid() };
        _paymentCreditCardFacadeMock
            .Setup(x => x.MakePayment(It.IsAny<Payment>()))
            .Returns(transaction);

        var unitOfWorkMock = new Mock<IUnitOfWork>();
        _paymentRepositoryMock.Setup(x => x.UnitOfWork).Returns(unitOfWorkMock.Object);

        // Act
        var result = await _paymentService.MakePaymentCourse(paymentCourse);

        // Assert
        result.Should().BeTrue();
        _encryptionServiceMock.Verify(x => x.Encrypt(paymentCourse.CardNumber), Times.Once);
        _encryptionServiceMock.Verify(x => x.Encrypt(paymentCourse.CardExpirationDate), Times.Once);
        _encryptionServiceMock.Verify(x => x.Encrypt(paymentCourse.CardCVV), Times.Once);
    }
}
