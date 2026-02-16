using AcademyIO.Courses.API.Application.Commands;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Models.ViewModels;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class ModelsCoverageTests
    {
        [Fact]
        public void LessonViewModel_Properties_Work()
        {
            var vm = new LessonViewModel
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Subject = "Sub",
                TotalHours = 10,
                CourseId = Guid.NewGuid()
            };

            Assert.NotEqual(Guid.Empty, vm.Id);
            Assert.Equal("Test", vm.Name);
            Assert.Equal("Sub", vm.Subject);
            Assert.Equal(10, vm.TotalHours);
            Assert.NotEqual(Guid.Empty, vm.CourseId);
        }

        [Fact]
        public void LessonProgressViewModel_Properties_Work()
        {
            var vm = new LessonProgressViewModel("L1", "InProgress");

            Assert.Equal("L1", vm.LessonName);
            Assert.Equal("InProgress", vm.ProgressLesson);
        }

        [Fact]
        public void CourseViewModel_Properties_Work()
        {
            var vm = new CourseViewModel
            {
                Id = Guid.NewGuid(),
                Name = "C1",
                Description = "D1",
                Price = 99.99,
            };

            Assert.NotEqual(Guid.Empty, vm.Id);
            Assert.Equal("C1", vm.Name);
            Assert.Equal("D1", vm.Description);
            Assert.Equal(99.99, vm.Price);
        }

        [Fact]
        public void PaymentViewModel_Properties_Work()
        {
            var vm = new PaymentViewModel
            {
                CardName = "Name",
                CardNumber = "1234",
                CardExpirationDate = "12/30",
                CardCVV = "123"
            };

            Assert.Equal("Name", vm.CardName);
            Assert.Equal("1234", vm.CardNumber);
            Assert.Equal("12/30", vm.CardExpirationDate);
            Assert.Equal("123", vm.CardCVV);
        }

        // Adicione outros ViewModels aqui se necessário
    }
}