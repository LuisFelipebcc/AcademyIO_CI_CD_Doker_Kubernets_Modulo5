using AcademyIO.Core.Enums;
using AcademyIO.Courses.API.Models;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class ProgressLessonModelTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            var progress = new ProgressLesson(lessonId, studentId, EProgressLesson.InProgress);

            Assert.Equal(lessonId, progress.LessonId);
            Assert.Equal(studentId, progress.StudentId);
            Assert.Equal(EProgressLesson.InProgress, progress.ProgressionStatus);
        }
    }
}
