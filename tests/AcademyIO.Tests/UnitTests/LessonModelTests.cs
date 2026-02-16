using AcademyIO.Courses.API.Models;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class LessonModelTests
    {
        [Fact]
        public void Constructor_SetsProperties()
        {
            var courseId = Guid.NewGuid();
            var lesson = new Lesson("LName", "Subj", 3.5, courseId);

            Assert.Equal("LName", lesson.Name);
            Assert.Equal("Subj", lesson.Subject);
            Assert.Equal(3.5, lesson.TotalHours);
            Assert.Equal(courseId, lesson.CourseId);
        }
    }
}
