using AcademyIO.Core.Enums;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Controllers;
using AcademyIO.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AcademyIO.Tests.Courses
{
    public class LessonsControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<ILessonQuery> _lessonQueryMock;
        private readonly Mock<IAspNetUser> _aspNetUserMock;
        private readonly LessonsController _controller;
        private readonly Guid _userId;

        public LessonsControllerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _lessonQueryMock = new Mock<ILessonQuery>();
            _aspNetUserMock = new Mock<IAspNetUser>();
            _userId = Guid.NewGuid();

            _aspNetUserMock.Setup(u => u.GetUserId()).Returns(_userId);

            _controller = new LessonsController(
                _mediatorMock.Object,
                _lessonQueryMock.Object,
                _aspNetUserMock.Object
            );
        }

        [Fact]
        public async Task GetAll_ShouldReturnLessons()
        {
            // Arrange
            _lessonQueryMock.Setup(q => q.GetAll()).ReturnsAsync(new List<LessonViewModel>());

            // Act
            await _controller.GetAll();

            // Assert
            _lessonQueryMock.Verify(q => q.GetAll(), Times.Once);
        }

        [Fact]
        public async Task GetByCourseId_ShouldReturnLessons()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.GetByCourseId(courseId)).ReturnsAsync(new List<LessonViewModel>());

            // Act
            await _controller.GetByCourseId(courseId);

            // Assert
            _lessonQueryMock.Verify(q => q.GetByCourseId(courseId), Times.Once);
        }

        [Fact]
        public async Task GetProgress_ShouldReturnProgress_WhenCalled()
        {
            // Arrange
            var progressList = new List<LessonProgressViewModel>();
            _lessonQueryMock.Setup(q => q.GetProgress(_userId)).ReturnsAsync(progressList);

            // Act
            await _controller.GetProgress();

            // Assert
            _lessonQueryMock.Verify(q => q.GetProgress(_userId), Times.Once);
        }

        [Fact]
        public async Task Add_ShouldSendCommand()
        {
            // Arrange
            var lesson = new LessonViewModel { Name = "Lesson 1", CourseId = Guid.NewGuid(), TotalHours = 10 };

            // Act
            await _controller.Add(lesson);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<AddLessonCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task StartClass_ShouldFail_WhenNotEnrolled()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, _userId)).Returns(false);

            // Act
            await _controller.StartClass(lessonId);

            // Assert
            // Verifica que o comando NÃO foi enviado
            _mediatorMock.Verify(m => m.Send(It.IsAny<StartLessonCommand>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task StartClass_ShouldFail_WhenAlreadyCompleted()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, _userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, _userId)).Returns(EProgressLesson.Completed);

            // Act
            await _controller.StartClass(lessonId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<StartLessonCommand>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task StartClass_ShouldSucceed_WhenValid()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, _userId)).Returns(true);
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, _userId)).Returns(EProgressLesson.NotStarted);

            // Act
            await _controller.StartClass(lessonId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<StartLessonCommand>(), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task FinishClass_ShouldFail_WhenNotStarted()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, _userId)).Returns(true);
            // Se o status for NotStarted, não pode finalizar
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, _userId)).Returns(EProgressLesson.NotStarted);

            // Act
            await _controller.FinishClass(lessonId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<FinishLessonCommand>(), CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task FinishClass_ShouldSucceed_WhenValid()
        {
            // Arrange
            var lessonId = Guid.NewGuid();
            _lessonQueryMock.Setup(q => q.ExistsProgress(lessonId, _userId)).Returns(true);
            // Status deve ser diferente de NotStarted (ex: InProgress) para poder finalizar
            _lessonQueryMock.Setup(q => q.GetProgressStatusLesson(lessonId, _userId)).Returns(EProgressLesson.InProgress);

            // Act
            await _controller.FinishClass(lessonId);

            // Assert
            _mediatorMock.Verify(m => m.Send(It.IsAny<FinishLessonCommand>(), CancellationToken.None), Times.Once);
        }
    }
}