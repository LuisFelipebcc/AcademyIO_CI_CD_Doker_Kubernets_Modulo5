using AcademyIO.Courses.API.Application.Commands;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
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
            Assert.Contains(command.ValidationResult.Errors, e => e.PropertyName == "CourseId");
        }

        [Fact]
        public void ValidatePaymentCourseCommand_ShouldBeValid_WhenDataIsCorrect()
        {
            var cmd = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "4111111111111111", "12/25", "123");

            var result = cmd.IsValid();

            Assert.True(result);
            Assert.Empty(cmd.ValidationResult.Errors);
        }

        [Fact]
        public void ValidatePaymentCourseCommand_ShouldBeInvalid_WhenFieldsAreEmpty()
        {
            var cmd = new ValidatePaymentCourseCommand(Guid.Empty, Guid.Empty, "", "", "", "");

            var result = cmd.IsValid();

            Assert.False(result);
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("Course"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("Student"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("Nome do Cartão"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("Número do Cartão"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("Expiração do Cartão"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("CVV"));
        }

        [Fact]
        public void ValidatePaymentCourseCommand_ShouldBeInvalid_WhenCardNumberIsInvalid()
        {
            var cmd = new ValidatePaymentCourseCommand(Guid.NewGuid(), Guid.NewGuid(), "John Doe", "invalid", "12/25", "123");

            var result = cmd.IsValid();

            Assert.False(result);
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("inválido"));
        }

        [Fact]
        public void CreateProgressByCourseCommand_ShouldBeValid_WhenDataIsCorrect()
        {
            var cmd = new CreateProgressByCourseCommand(Guid.NewGuid(), Guid.NewGuid());

            var result = cmd.IsValid();

            Assert.True(result);
            Assert.Empty(cmd.ValidationResult.Errors);
        }

        [Fact]
        public void CreateProgressByCourseCommand_ShouldBeInvalid_WhenIdsAreEmpty()
        {
            var cmd = new CreateProgressByCourseCommand(Guid.Empty, Guid.Empty);

            var result = cmd.IsValid();

            Assert.False(result);
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("curso"));
            Assert.Contains(cmd.ValidationResult.Errors, e => e.ErrorMessage.Contains("aluno"));
        }
    }
}