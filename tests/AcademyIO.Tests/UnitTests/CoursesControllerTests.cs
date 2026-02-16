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
using System.Net;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;

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

        [Fact]
        public async Task GetById_ShouldReturnOk_WhenCourseExists()
        {
            var id = Guid.NewGuid();
            var vm = new CourseViewModel { Id = id, Name = "Course" };
            _courseQueryMock.Setup(q => q.GetById(id)).ReturnsAsync(vm);

            var result = await _controller.GetById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(vm, ok.Value);
            _courseQueryMock.Verify(q => q.GetById(id), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnOk_WithNull_WhenCourseNotFound()
        {
            var id = Guid.NewGuid();
            _courseQueryMock.Setup(q => q.GetById(id)).ReturnsAsync((CourseViewModel)null);

            var result = await _controller.GetById(id);

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Null(ok.Value);
            _courseQueryMock.Verify(q => q.GetById(id), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldMapAllFields_AndReturnCreated()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Name = "N", Description = "D", Price = 123.45 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddCourseCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.Create(vm);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.Created, ok.Value);
            _mediatorMock.Verify(m => m.Send(
                It.Is<AddCourseCommand>(c => c.Name == vm.Name && c.Description == vm.Description && c.Price == vm.Price && c.UserCreationId == userId),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldMapAllFields_AndReturnNoContent()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Id = Guid.NewGuid(), Name = "N2", Description = "D2", Price = 88 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.Update(vm);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(
                It.Is<UpdateCourseCommand>(c => c.Name == vm.Name && c.Description == vm.Description && c.Price == vm.Price && c.CourseId == vm.Id && c.UserCreationId == userId),
                It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public void AuthorizationAttributes_ShouldBeConfiguredCorrectly_ForCourses()
        {
            var t = typeof(CoursesController);
            var getAll = t.GetMethod("GetAll");
            var getById = t.GetMethod("GetById");
            var create = t.GetMethod("Create");
            var update = t.GetMethod("Update");
            var remove = t.GetMethod("Remove");
            var makePayment = t.GetMethod("MakePayment");

            Assert.NotNull(getAll.GetCustomAttribute<AllowAnonymousAttribute>());
            Assert.NotNull(getById.GetCustomAttribute<AllowAnonymousAttribute>());

            var createAuth = create.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(createAuth);
            Assert.Equal("ADMIN", createAuth.Roles);

            var updateAuth = update.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(updateAuth);
            Assert.Equal("ADMIN", updateAuth.Roles);

            var removeAuth = remove.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(removeAuth);
            Assert.Equal("ADMIN", removeAuth.Roles);

            var payAuth = makePayment.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(payAuth);
            Assert.Equal("STUDENT", payAuth.Roles);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WithEmptyList()
        {
            _courseQueryMock.Setup(q => q.GetAll()).ReturnsAsync(new List<CourseViewModel>());

            var result = await _controller.GetAll();

            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var list = Assert.IsAssignableFrom<IEnumerable<CourseViewModel>>(ok.Value);
            Assert.Empty(list);
            _courseQueryMock.Verify(q => q.GetAll(), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldSendCommandWithCorrectId()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveCourseCommand>(), default)).ReturnsAsync(true);

            var result = await _controller.Remove(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<RemoveCourseCommand>(c => c.CourseId == id), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldUseAuthenticatedUserId_Once()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Name = "N", Description = "D", Price = 10 };

            await _controller.Create(vm);

            _userMock.Verify(u => u.GetUserId(), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldUseAuthenticatedUserId_Once()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Id = Guid.NewGuid(), Name = "N", Description = "D", Price = 10 };

            await _controller.Update(vm);

            _userMock.Verify(u => u.GetUserId(), Times.Once);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakePayment_ShouldRethrow_WhenBusThrows()
        {
            var courseId = Guid.NewGuid();
            var vm = new PaymentViewModel { CardName = "X", CardNumber = "4111111111111111", CardExpirationDate = "12/25", CardCVV = "123" };
            _userMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());
            _busMock.Setup(b => b.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()))
                .ThrowsAsync(new InvalidOperationException("bus error"));

            await Assert.ThrowsAsync<InvalidOperationException>(() => _controller.MakePayment(courseId, vm));
        }

        [Fact]
        public async Task Create_ShouldStillReturnCreated_WhenMediatorReturnsFalse()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Name = "X", Description = "Y", Price = 10 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Create(vm);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.Created, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldStillReturnNoContent_WhenMediatorReturnsFalse()
        {
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            var vm = new CourseViewModel { Id = Guid.NewGuid(), Name = "X", Description = "Y", Price = 10 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Update(vm);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldStillReturnNoContent_WhenMediatorReturnsFalse()
        {
            var id = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<RemoveCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()))
                .ReturnsAsync(false);

            var result = await _controller.Remove(id);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveCourseCommand>(), It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        }
    }
}
