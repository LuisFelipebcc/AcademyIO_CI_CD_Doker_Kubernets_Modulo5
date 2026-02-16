using AcademyIO.Core.Data;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Handlers;
using AcademyIO.Courses.API.Models;
using MediatR;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class LessonCommandHandlerTests
    {
        private readonly Mock<ILessonRepository> _lessonRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly LessonCommandHandler _handler;

        public LessonCommandHandlerTests()
        {
            _lessonRepoMock = new Mock<ILessonRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mediatorMock = new Mock<IMediator>();

            _lessonRepoMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _handler = new LessonCommandHandler(_lessonRepoMock.Object, _mediatorMock.Object);
        }

        // AddLessonCommand Tests
        [Fact]
        public async Task Handle_AddLessonCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var courseId = Guid.NewGuid();
            var command = new AddLessonCommand("Lesson 1", "Subject 1", courseId, 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _lessonRepoMock.Verify(r => r.Add(It.IsAny<Lesson>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AddLessonCommand_WithInvalidCommand_PublishesNotificationAndReturnsFalse()
        {
            var command = new AddLessonCommand("", "", Guid.Empty, 0);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.Add(It.IsAny<Lesson>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_AddLessonCommand_WhenCommitFails_ReturnsFalse()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var command = new AddLessonCommand("Lesson 1", "Subject 1", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.Add(It.IsAny<Lesson>()), Times.Once);
        }

        [Fact]
        public async Task Handle_AddLessonCommand_WithEmptyName_ReturnsFalse()
        {
            var command = new AddLessonCommand("", "Valid Subject", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_AddLessonCommand_WithEmptySubject_ReturnsFalse()
        {
            var command = new AddLessonCommand("Valid Lesson", "", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        // StartLessonCommand Tests
        [Fact]
        public async Task Handle_StartLessonCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new StartLessonCommand(lessonId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _lessonRepoMock.Verify(r => r.StartLesson(lessonId, studentId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_StartLessonCommand_WithInvalidCommand_PublishesNotificationAndReturnsFalse()
        {
            var command = new StartLessonCommand(Guid.Empty, Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.StartLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_StartLessonCommand_WithEmptyLessonId_ReturnsFalse()
        {
            var command = new StartLessonCommand(Guid.Empty, Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_StartLessonCommand_WithEmptyStudentId_ReturnsFalse()
        {
            var command = new StartLessonCommand(Guid.NewGuid(), Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_StartLessonCommand_WhenCommitFails_ReturnsFalse()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new StartLessonCommand(lessonId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.StartLesson(lessonId, studentId), Times.Once);
        }

        // FinishLessonCommand Tests
        [Fact]
        public async Task Handle_FinishLessonCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new FinishLessonCommand(lessonId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _lessonRepoMock.Verify(r => r.FinishLesson(lessonId, studentId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_FinishLessonCommand_WithInvalidCommand_PublishesNotificationAndReturnsFalse()
        {
            var command = new FinishLessonCommand(Guid.Empty, Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.FinishLesson(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Handle_FinishLessonCommand_WithEmptyLessonId_ReturnsFalse()
        {
            var command = new FinishLessonCommand(Guid.Empty, Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_FinishLessonCommand_WithEmptyStudentId_ReturnsFalse()
        {
            var command = new FinishLessonCommand(Guid.NewGuid(), Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_FinishLessonCommand_WhenCommitFails_ReturnsFalse()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var command = new FinishLessonCommand(lessonId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.FinishLesson(lessonId, studentId), Times.Once);
        }
    }
}
