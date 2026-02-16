using System;
using System.Threading;
using System.Threading.Tasks;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Handlers;
using AcademyIO.Courses.API.Models;
using AcademyIO.Core.Data;
using AcademyIO.Core.Messages.IntegrationCommands;
using MediatR;
using Moq;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CourseCommandHandlerTests
    {
        private readonly Mock<ICourseRepository> _courseRepoMock;
        private readonly Mock<ILessonRepository> _lessonRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly CourseCommandHandler _handler;

        public CourseCommandHandlerTests()
        {
            _courseRepoMock = new Mock<ICourseRepository>();
            _lessonRepoMock = new Mock<ILessonRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mediatorMock = new Mock<IMediator>();

            _courseRepoMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _lessonRepoMock.SetupGet(r => r.UnitOfWork).Returns(_unitOfWorkMock.Object);
            _mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _handler = new CourseCommandHandler(_courseRepoMock.Object, _lessonRepoMock.Object, _mediatorMock.Object);
        }

        // AddCourseCommand Tests
        [Fact]
        public async Task Handle_AddCourseCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var command = new AddCourseCommand("Course 1", "Description", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AddCourseCommand_WithInvalidCommand_PublishesNotification_ReturnsFalse()
        {
            var command = new AddCourseCommand("", "", Guid.Empty, 0);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_AddCourseCommand_WithEmptyName_ReturnsFalse()
        {
            var command = new AddCourseCommand("", "Valid Description", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Handle_AddCourseCommand_WithNegativePrice_ReturnsFalse()
        {
            var command = new AddCourseCommand("Valid Course", "Valid Description", Guid.NewGuid(), -5);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Never);
        }

        // UpdateCourseCommand Tests
        [Fact]
        public async Task Handle_UpdateCourseCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            var courseId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var existingCourse = new Course { Id = courseId, Name = "Old Name", Description = "Old Desc", Price = 5 };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(existingCourse);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var command = new UpdateCourseCommand("New Name", "New Desc", userId, 20, courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _courseRepoMock.Verify(r => r.Update(It.IsAny<Course>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateCourseCommand_WithCourseNotFound_PublishesNotificationAndReturnsFalse()
        {
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync((Course)null);

            var command = new UpdateCourseCommand("Name", "Desc", Guid.NewGuid(), 20, courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Update(It.IsAny<Course>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateCourseCommand_WithInvalidCommand_PublishesNotificationAndReturnsFalse()
        {
            var command = new UpdateCourseCommand("", "", Guid.Empty, -5, Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            _courseRepoMock.Verify(r => r.Update(It.IsAny<Course>()), Times.Never);
        }

        [Fact]
        public async Task Handle_UpdateCourseCommand_WithEmptyName_ReturnsFalse()
        {
            var command = new UpdateCourseCommand("", "Valid Desc", Guid.NewGuid(), 20, Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        // RemoveCourseCommand Tests
        [Fact]
        public async Task Handle_RemoveCourseCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            var courseId = Guid.NewGuid();
            var existingCourse = new Course { Id = courseId, Name = "Course to Delete" };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(existingCourse);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var command = new RemoveCourseCommand(courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _courseRepoMock.Verify(r => r.Delete(existingCourse), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_RemoveCourseCommand_WithCourseNotFound_PublishesNotificationAndReturnsFalse()
        {
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync((Course)null);

            var command = new RemoveCourseCommand(courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Delete(It.IsAny<Course>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RemoveCourseCommand_WithEmptyCourseId_PublishesNotificationAndReturnsFalse()
        {
            var command = new RemoveCourseCommand(Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
            _courseRepoMock.Verify(r => r.Delete(It.IsAny<Course>()), Times.Never);
        }

        // ValidatePaymentCourseCommand Tests
        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithValidCommand_SendsMakePaymentCommandAndReturnsTrue()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Course", Price = 100 };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);
            _mediatorMock.Setup(m => m.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var command = new ValidatePaymentCourseCommand(courseId, studentId, "John Doe", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithCourseNotFound_PublishesNotificationAndReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync((Course)null);

            var command = new ValidatePaymentCourseCommand(courseId, studentId, "John Doe", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithInvalidCardNumber_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "invalid", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _mediatorMock.Verify(m => m.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithEmptyCourseId_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.Empty, Guid.NewGuid(), "John Doe", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithEmptyStudentId_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.Empty, "John Doe", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithEmptyCardName_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithEmptyExpirationDate_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "4111111111111111", "", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WithEmptyCVV_ReturnsFalse()
        {
            var command = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "4111111111111111", "12/25", "");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        // CreateProgressByCourseCommand Tests
        [Fact]
        public async Task Handle_CreateProgressByCourseCommand_WithValidCommand_CommitsAndReturnsTrue()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.CreateProgressLessonByCourse(courseId, studentId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);

            var command = new CreateProgressByCourseCommand(courseId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            _lessonRepoMock.Verify(r => r.CreateProgressLessonByCourse(courseId, studentId), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_CreateProgressByCourseCommand_WithInvalidCommand_PublishesNotificationAndReturnsFalse()
        {
            var command = new CreateProgressByCourseCommand(Guid.Empty, Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _lessonRepoMock.Verify(r => r.CreateProgressLessonByCourse(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_CreateProgressByCourseCommand_WithEmptyCourseId_ReturnsFalse()
        {
            var command = new CreateProgressByCourseCommand(Guid.Empty, Guid.NewGuid());

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_CreateProgressByCourseCommand_WithEmptyStudentId_ReturnsFalse()
        {
            var command = new CreateProgressByCourseCommand(Guid.NewGuid(), Guid.Empty);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_AddCourseCommand_WhenCommitFails_ReturnsFalse()
        {
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var command = new AddCourseCommand("Course 1", "Description", Guid.NewGuid(), 10);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateCourseCommand_WhenCommitFails_ReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Existing Course" };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var command = new UpdateCourseCommand("New Name", "New Desc", Guid.NewGuid(), 20, courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Update(It.IsAny<Course>()), Times.Once);
        }

        [Fact]
        public async Task Handle_RemoveCourseCommand_WhenCommitFails_ReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var command = new RemoveCourseCommand(courseId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            _courseRepoMock.Verify(r => r.Delete(course), Times.Once);
        }

        [Fact]
        public async Task Handle_CreateProgressByCourseCommand_WhenCommitFails_ReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.CreateProgressLessonByCourse(courseId, studentId)).ReturnsAsync(true);
            _unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(false);

            var command = new CreateProgressByCourseCommand(courseId, studentId);

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }

        [Fact]
        public async Task Handle_ValidatePaymentCourseCommand_WhenPaymentFails_ReturnsFalse()
        {
            var courseId = Guid.NewGuid();
            var studentId = Guid.NewGuid();
            var course = new Course { Id = courseId, Price = 100 };

            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);
            _mediatorMock.Setup(m => m.Send(It.IsAny<MakePaymentCourseCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var command = new ValidatePaymentCourseCommand(courseId, studentId, "John Doe", "4111111111111111", "12/25", "123");

            var result = await _handler.Handle(command, CancellationToken.None);

            Assert.False(result);
        }
    }
}
