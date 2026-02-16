using AcademyIO.Core.Enums;
using AcademyIO.Courses.API.Data;
using AcademyIO.Courses.API.Data.Repository;
using AcademyIO.Courses.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class LessonRepositoryTests
    {
        private CoursesContext CreateContext(string dbName)
        {
            var mediator = new Mock<IMediator>();
            var options = new DbContextOptionsBuilder<CoursesContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new CoursesContext(options, mediator.Object);
        }

        [Fact]
        public async Task CreateProgressByCourse_Start_Finish_GetProgression_Workflow()
        {
            var db = CreateContext(Guid.NewGuid().ToString());
            var repo = new LessonRepository(db);

            var courseId = Guid.NewGuid();
            var lesson1 = new Lesson("L1", "Subj1", 1, courseId);
            var lesson2 = new Lesson("L2", "Subj2", 2, courseId);

            db.Lessons.Add(lesson1);
            db.Lessons.Add(lesson2);
            await db.SaveChangesAsync();

            var studentId = Guid.NewGuid();
            var created = await repo.CreateProgressLessonByCourse(courseId, studentId);
            Assert.True(created);

            // verify progress created
            var progress = (await repo.GetProgression(studentId)).ToList();
            Assert.Equal(2, progress.Count);
            Assert.All(progress, p => Assert.Equal(EProgressLesson.NotStarted, p.ProgressionStatus));

            // Start first lesson
            var startResult = await repo.StartLesson(lesson1.Id, studentId);
            Assert.True(startResult);
            await db.SaveChangesAsync();

            var statusAfterStart = repo.GetProgressStatusLesson(lesson1.Id, studentId);
            Assert.Equal(EProgressLesson.InProgress, statusAfterStart);

            // Finish first lesson
            var finishResult = await repo.FinishLesson(lesson1.Id, studentId);
            Assert.True(finishResult);
            await db.SaveChangesAsync();

            var statusAfterFinish = repo.GetProgressStatusLesson(lesson1.Id, studentId);
            Assert.Equal(EProgressLesson.Completed, statusAfterFinish);
        }
    }
}
