using AcademyIO.Courses.API.Application.Commands;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CommandValidationTests
    {
        [Fact]
        public void AddCourseCommand_InvalidWhenMissingFields()
        {
            var cmd = new AddCourseCommand("", "", Guid.Empty, 0);
            Assert.False(cmd.IsValid());
            Assert.Contains(cmd.ValidationResult.Errors, e => e.PropertyName == "Name");
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

        [Fact]
        public void StartLessonCommand_InvalidWhenMissingIds()
        {
            var cmd = new StartLessonCommand(Guid.Empty, Guid.Empty);
            Assert.False(cmd.IsValid());
            Assert.Contains(cmd.ValidationResult.Errors, e => e.PropertyName == "LessonId");
            Assert.Contains(cmd.ValidationResult.Errors, e => e.PropertyName == "StudentId");
        }

        [Fact]
        public void StartLessonCommand_ValidWhenIdsPresent()
        {
            var cmd = new StartLessonCommand(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(cmd.IsValid());
        }

        [Fact]
        public void FinishLessonCommand_InvalidWhenMissingIds()
        {
            var cmd = new FinishLessonCommand(Guid.Empty, Guid.Empty);
            Assert.False(cmd.IsValid());
        }

        [Fact]
        public void FinishLessonCommand_ValidWhenIdsPresent()
        {
            var cmd = new FinishLessonCommand(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(cmd.IsValid());
        }
    }
}
