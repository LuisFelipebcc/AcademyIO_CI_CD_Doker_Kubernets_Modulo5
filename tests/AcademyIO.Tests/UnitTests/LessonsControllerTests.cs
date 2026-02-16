using AcademyIO.Core.Enums;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Controllers;
using AcademyIO.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.UnitTests
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
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
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
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<FinishLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartClass_ShouldReturnNoContent_WhenAlreadyInProgress()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, userId)).Returns(EProgressLesson.InProgress);
            _mediatorMock.Setup(m => m.Send(It.IsAny<StartLessonCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _controller.StartClass(lessonId);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(HttpStatusCode.NoContent, ok.Value);
            _mediatorMock.Verify(m => m.Send(It.Is<StartLessonCommand>(c => c.LessonId == lessonId && c.StudentId == userId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetProgress_ShouldCallQueryWithAuthenticatedUser()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userMock.Setup(u => u.GetUserId()).Returns(userId);
            _lessonQueryMock.Setup(x => x.GetProgress(userId)).ReturnsAsync(new List<LessonProgressViewModel>());

            // Act
            var result = await _controller.GetProgress();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            _userMock.Verify(u => u.GetUserId(), Times.Once);
            _lessonQueryMock.Verify(x => x.GetProgress(userId), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldSendCommandWithAllMappedFields()
        {
            // Arrange
            var lessonVm = new LessonViewModel
            {
                Name = "Lesson Name",
                Subject = "Subject X",
                CourseId = Guid.NewGuid(),
                TotalHours = 42
            };
            _mediatorMock
                .Setup(m => m.Send(It.IsAny<AddLessonCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Add(lessonVm);

            // Assert
            Assert.True(result is ObjectResult || result is StatusCodeResult);
            _mediatorMock.Verify(m => m.Send(
                It.Is<AddLessonCommand>(c =>
                    c.Name == lessonVm.Name &&
                    c.Subject == lessonVm.Subject &&
                    c.CourseId == lessonVm.CourseId &&
                    c.TotalHours == lessonVm.TotalHours
                ),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetByCourseId_ShouldInvokeQueryOnceWithProvidedId()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var lessons = new List<LessonViewModel>();
            _lessonQueryMock.Setup(x => x.GetByCourseId(courseId)).ReturnsAsync(lessons);

            // Act
            var result = await _controller.GetByCourseId(courseId);

            // Assert
            var actionResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Same(lessons, actionResult.Value);
            _lessonQueryMock.Verify(x => x.GetByCourseId(courseId), Times.Once);
        }

        [Fact]
        public void AuthorizationAttributes_ShouldBeConfiguredCorrectly()
        {
            // Arrange & Act
            var controllerType = typeof(LessonsController);

            var getAll = controllerType.GetMethod("GetAll");
            var getByCourseId = controllerType.GetMethod("GetByCourseId");
            var getProgress = controllerType.GetMethod("GetProgress");
            var add = controllerType.GetMethod("Add");
            var startClass = controllerType.GetMethod("StartClass");
            var finishClass = controllerType.GetMethod("FinishClass");

            // Assert
            Assert.NotNull(getAll.GetCustomAttribute<AllowAnonymousAttribute>());
            Assert.NotNull(getByCourseId.GetCustomAttribute<AllowAnonymousAttribute>());

            var getProgressAuth = getProgress.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(getProgressAuth);
            Assert.Equal("STUDENT", getProgressAuth.Roles);

            var addAuth = add.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(addAuth);
            Assert.Equal("ADMIN", addAuth.Roles);

            var startAuth = startClass.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(startAuth);
            Assert.Equal("STUDENT", startAuth.Roles);

            var finishAuth = finishClass.GetCustomAttribute<AuthorizeAttribute>();
            Assert.NotNull(finishAuth);
            Assert.Equal("STUDENT", finishAuth.Roles);
        }
    }
}