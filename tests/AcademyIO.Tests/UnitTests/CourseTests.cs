using AcademyIO.Courses.API.Models;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CourseTests
    {
        [Fact]
        public void AddLesson_Null_ThrowsArgumentNullException()
        {
            var course = new Course();
            Assert.Throws<ArgumentNullException>(() => course.AddLesson(null!));
        }

        [Fact]
        public void AddLesson_Duplicate_ThrowsInvalidOperationException()
        {
            var course = new Course();
            var lesson = new Lesson("Name", "Subject", 1.0, course.Id);
            course.AddLesson(lesson);
            Assert.Throws<InvalidOperationException>(() => course.AddLesson(lesson));
        }

        [Fact]
        public void AddLesson_AddsSuccessfully()
        {
            var course = new Course();
            var lesson = new Lesson("Name", "Subject", 1.0, course.Id);
            course.AddLesson(lesson);
            Assert.Single(course.Lessons);
        }
    }
}
