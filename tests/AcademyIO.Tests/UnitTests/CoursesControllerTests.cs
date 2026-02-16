using AcademyIO.Core.Messages.Integration;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Controllers;
using AcademyIO.Courses.API.Models.ViewModels;
using AcademyIO.MessageBus;
using AcademyIO.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace AcademyIO.Tests.UnitTests
{
    public class CoursesControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ICourseQuery> _courseQueryMock;
        private readonly Mock<IAspNetUser> _aspNetUserMock;
        private readonly Mock<IMessageBus> _messageBusMock;
        private readonly CoursesController _controller;

        public CoursesControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _courseQueryMock = new Mock<ICourseQuery>();
            _aspNetUserMock = new Mock<IAspNetUser>();
            _messageBusMock = new Mock<IMessageBus>();

            _aspNetUserMock.Setup(u => u.GetUserId()).Returns(Guid.NewGuid());

            _controller = new CoursesController(
                _mediatorMock.Object,
                _courseQueryMock.Object,
                _aspNetUserMock.Object,
                _messageBusMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCalled()
        {
            // Arrange
            var courses = new List<CourseViewModel> { new CourseViewModel { Id = Guid.NewGuid(), Name = "Test Course" } };
            _courseQueryMock.Setup(q => q.GetAll()).ReturnsAsync(courses);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CourseViewModel>>>(result);
            _courseQueryMock.Verify(q => q.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnCourse_WhenExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseViewModel { Id = courseId, Name = "Test Course" };
            _courseQueryMock.Setup(q => q.GetById(courseId)).ReturnsAsync(course);

            // Act
            var result = await _controller.GetById(courseId);

            // Assert
            _courseQueryMock.Verify(q => q.GetById(courseId), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotExists()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _courseQueryMock.Setup(q => q.GetById(courseId)).ReturnsAsync((CourseViewModel)null);

            // Act
            var result = await _controller.GetById(courseId);

            // Assert
            _courseQueryMock.Verify(q => q.GetById(courseId), Times.Once);
        }

        [Fact]
        public async Task Create_ShouldSendCommand_AndReturnCreated()
        {
            // Arrange
            var courseVm = new CourseViewModel { Name = "New Course", Description = "Desc", Price = 100 };

            // Act
            var result = await _controller.Create(courseVm);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddCourseCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Update_ShouldSendCommand_AndReturnNoContent()
        {
            // Arrange
            var courseVm = new CourseViewModel { Id = Guid.NewGuid(), Name = "Updated Course", Description = "Desc", Price = 150 };

            // Act
            var result = await _controller.Update(courseVm);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<UpdateCourseCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Remove_ShouldSendCommand_AndReturnNoContent()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act
            var result = await _controller.Remove(courseId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<RemoveCourseCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task MakePayment_ShouldRequestBus_WhenCalled()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var paymentVm = new PaymentViewModel { CardName = "Test", CardNumber = "123", CardExpirationDate = "10/30", CardCVV = "123" };

            // Act
            await _controller.MakePayment(courseId, paymentVm);

            // Assert
            _messageBusMock.Verify(b => b.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(It.IsAny<PaymentRegisteredIntegrationEvent>()), Times.Once);
        }
    }
}