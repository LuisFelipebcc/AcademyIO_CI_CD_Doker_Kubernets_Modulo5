using System;
using Xunit;
using AcademyIO.Courses.API.Application.Commands;

namespace AcademyIO.Tests.UnitTests
{
    public class CommandValidationTests
    {
        [Fact]
        public void AddCourseCommand_InvalidWhenMissingFields()
        {
            var cmd = new AddCourseCommand("", "", Guid.Empty, 0);
            Assert.False(cmd.IsValid());
            Assert.Contains(AddCourseCommandValidation.NameError, cmd.ValidationResult.Errors[0].ErrorMessage);
        }

        [Fact]
        public void AddCourseCommand_ValidWhenAllFieldsPresent()
        {
            var cmd = new AddCourseCommand("Name", "Desc", Guid.NewGuid(), 10.0);
            Assert.True(cmd.IsValid());
        }

        [Fact]
        public void AddLessonCommand_InvalidWhenMissingFields()
        {
            var cmd = new AddLessonCommand("", "", Guid.Empty, 0);
            Assert.False(cmd.IsValid());
        }

        [Fact]
        public void AddLessonCommand_ValidWhenAllFieldsPresent()
        {
            var cmd = new AddLessonCommand("Name", "Subject", Guid.NewGuid(), 1.5);
            Assert.True(cmd.IsValid());
        }
    }
}
