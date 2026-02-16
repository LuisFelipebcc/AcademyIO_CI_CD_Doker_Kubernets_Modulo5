using AcademyIO.Courses.API.Application.Commands;
using System;
using Xunit;

namespace AcademyIO.Tests.Courses.Commands
{
    public class CourseCommandsTests
    {
        [Fact]
        public void AddCourseCommand_ShouldBeValid_WhenDataIsCorrect()
        {
            // Arrange
            var command = new AddCourseCommand("Curso .NET", "Descrição completa", Guid.NewGuid(), 199.99);

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void AddCourseCommand_ShouldBeInvalid_WhenDataIsIncorrect()
        {
            // Arrange
            var command = new AddCourseCommand("", "", Guid.Empty, 0);

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "Name");
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "Description");
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "Price");
        }

        [Fact]
        public void UpdateCourseCommand_ShouldBeValid_WhenDataIsCorrect()
        {
            // Arrange
            var command = new UpdateCourseCommand("Curso .NET Avançado", "Nova Descrição", Guid.NewGuid(), 250.00, Guid.NewGuid());

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void UpdateCourseCommand_ShouldBeInvalid_WhenDataIsIncorrect()
        {
            // Arrange
            var command = new UpdateCourseCommand("", "", Guid.Empty, 0, Guid.Empty);

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "Name");
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "Id");
        }
    }
}