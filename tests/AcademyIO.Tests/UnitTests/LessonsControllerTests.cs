using AcademyIO.Core.Enums;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Controllers;
using AcademyIO.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.Courses.Controllers
{
    public class LessonsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IAspNetUser> _userMock;
        private readonly LessonsController _controller;

        public LessonsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _userMock = new Mock<IAspNetUser>();
            _controller = new LessonsController(_mediatorMock.Object, _lessonQueryMock.Object, _userMock.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenCalled()
        {
            // Arrange
            var lessons = new List<LessonViewModel> { new LessonViewModel { Id = Guid.NewGuid(), Name = "Test Lesson" } };
            _lessonQueryMock.Setup(x => x.GetAll()).ReturnsAsync(lessons);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnLessons = Assert.IsAssignableFrom<IEnumerable<LessonViewModel>>(actionResult.Value);
            Assert.Single(returnLessons);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnOk_WhenCalled()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessons = new List<LessonViewModel> { new LessonViewModel { Id = Guid.NewGuid(), CourseId = courseId } };
            _lessonQueryMock.Setup(x => x.GetByCourseId(courseId)).ReturnsAsync(lessons);

            // Act
            var result = await _controller.GetByCourseId(courseId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnLessons = Assert.IsAssignableFrom<IEnumerable<LessonViewModel>>(actionResult.Value);
            Assert.Single(returnLessons);
        }

        [Fact]
        public async Task Add_ShouldReturnCreated_WhenCommandIsValid()
        {
            // Arrange
            var lessonVm = new LessonViewModel { Name = "New Lesson", Subject = "Testing", CourseId = Guid.NewGuid(), TotalHours = 10 };
            _mediatorMock.Setup(m => m.Send(It.IsAny<AddLessonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.Add(lessonVm);

            // Assert
            // Nota: O controller retorna CustomResponse(HttpStatusCode.Created), que pode variar dependendo da implementação do MainController.
            // Geralmente é um ObjectResult ou StatusCodeResult.
            Assert.True(result is ObjectResult || result is StatusCodeResult);
            _mediatorMock.Verify(m => m.Send(It.Is<AddLessonCommand>(c => c.Name == lessonVm.Name), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartClass_ShouldReturnError_WhenUserNotEnrolled()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(false);

            // Act
            var result = await _controller.StartClass(lessonId);

            // Assert
            // Espera-se que o CustomResponse retorne BadRequest ou similar quando há erros na stack
            Assert.IsType<BadRequestObjectResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<StartLessonCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task StartClass_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.NotStarted);
            _mediatorMock.Setup(m => m.Send(It.IsAny<StartLessonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.StartClass(lessonId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<StartLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnOk_WhenCalled()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var progress = new List<LessonProgressViewModel>();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(x => x.GetProgress(userId)).ReturnsAsync(progress);

            // Act
            var result = await _controller.GetProgress();

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result);
            Assert.Same(progress, actionResult.Value);
        }

        [Fact]
        public async Task StartClass_ShouldReturnError_WhenAlreadyCompleted()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.Completed);

            // Act
            var result = await _controller.StartClass(lessonId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<StartLessonCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task FinishClass_ShouldReturnError_WhenUserNotEnrolled()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(false);

            // Act
            var result = await _controller.FinishClass(lessonId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<FinishLessonCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task FinishClass_ShouldReturnError_WhenNotStarted()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.NotStarted);

            // Act
            var result = await _controller.FinishClass(lessonId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<FinishLessonCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task FinishClass_ShouldReturnNoContent_WhenSuccess()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.InProgress);
            _mediatorMock.Setup(m => m.Send(It.IsAny<FinishLessonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.FinishClass(lessonId);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mediatorMock.Verify(m => m.Send(It.Is<FinishLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}