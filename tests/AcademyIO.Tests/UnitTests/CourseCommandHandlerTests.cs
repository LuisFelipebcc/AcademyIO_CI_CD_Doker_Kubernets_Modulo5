using System;
using System.Threading;
using System.Threading.Tasks;
using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Handlers;
using AcademyIO.Courses.API.Models;
using AcademyIO.Core.Data;
using MediatR;
using Moq;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CourseCommandHandlerTests
    {
        [Fact]
        public async Task Handle_AddCourseCommand_CommitsAndReturnsTrue()
        {
            var courseRepoMock = new Mock<ICourseRepository>();
            var lessonRepoMock = new Mock<ILessonRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            unitOfWorkMock.Setup(u => u.Commit()).ReturnsAsync(true);
            courseRepoMock.SetupGet(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

            var mediatorMock = new Mock<IMediator>();

            var handler = new CourseCommandHandler(courseRepoMock.Object, lessonRepoMock.Object, mediatorMock.Object);

            var command = new AddCourseCommand("Course 1", "Description", Guid.NewGuid(), 10);

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.True(result);
            courseRepoMock.Verify(r => r.Add(It.IsAny<Course>()), Times.Once);
            unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_UpdateCourseCommand_PublishesNotificationWhenNotFound_ReturnsFalse()
        {
            var courseRepoMock = new Mock<ICourseRepository>();
            var lessonRepoMock = new Mock<ILessonRepository>();
            var unitOfWorkMock = new Mock<IUnitOfWork>();
            courseRepoMock.SetupGet(r => r.UnitOfWork).Returns(unitOfWorkMock.Object);

            courseRepoMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((Course)null);

            var mediatorMock = new Mock<IMediator>();
            mediatorMock.Setup(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()))
                      .Returns(Task.CompletedTask)
                      .Verifiable();

            var handler = new CourseCommandHandler(courseRepoMock.Object, lessonRepoMock.Object, mediatorMock.Object);

            var command = new UpdateCourseCommand("Name", "Desc", Guid.NewGuid(), 20, Guid.NewGuid());

            var result = await handler.Handle(command, CancellationToken.None);

            Assert.False(result);
            mediatorMock.Verify(m => m.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }
    }
}
