using AcademyIO.Courses.API.Controllers;
using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Models.ViewModels;
using AcademyIO.Core.Messages.Integration;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.MessageBus;
using AcademyIO.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation.Results;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CoursesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IAspNetUser> _userMock;
        private readonly Mock<IMessageBus> _busMock;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _courseQueryMock = new Mock<ICourseQuery>();
            _userMock = new Mock<IAspNetUser>();
            _busMock = new Mock<IMessageBus>();
            _controller = new CoursesController(_mediatorMock.Object, _courseQueryMock.Object, _userMock.Object, _busMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCalled()
        {
            var list = new List<CourseViewModel> { new CourseViewModel { Id = Guid.NewGuid(), Name = "C1" } };
            _courseQueryMock.Setup(q => q.GetAll()).ReturnsAsync(list);

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<CourseViewModel>>(ok.Value);
        }

        [Fact]
        public async Task Create_Update_Remove_ReturnsExpectedCustomResponses()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);

            var vm = new CourseViewModel { Id = Guid.NewGuid(), Name = "New", Description = "D", Price = 10 };

            var createResult = await _controller.Create(vm);
            var createOk = Assert.IsType<OkObjectResult>(createResult);
            Assert.Equal(System.Net.HttpStatusCode.Created, createOk.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            var updateResult = await _controller.Update(vm);
            var updateOk = Assert.IsType<OkObjectResult>(updateResult);
            Assert.Equal(System.Net.HttpStatusCode.NoContent, updateOk.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);

            var removeResult = await _controller.Remove(vm.Id);
            var removeOk = Assert.IsType<OkObjectResult>(removeResult);
            Assert.Equal(System.Net.HttpStatusCode.NoContent, removeOk.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakePayment_InvokesMessageBus_ReturnsResponse()
        {
            var vm = new PaymentViewModel { CardName = "John", CardNumber = "4111", CardExpirationDate = "12/25", CardCVV = "123" };
            var courseId = Guid.NewGuid();
            var response = new ResponseMessage(new ValidationResult());

            _userMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
            _busMock.Setup(b => b.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()))
                .ReturnsAsync(response);

            var result = await _controller.MakePayment(courseId, vm);

            Assert.Same(response, result);
            _busMock.Verify(b => b.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()), Times.Once);
        }
    }
}
