using AcademyIO.Courses.API.Data;
using AcademyIO.Courses.API.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CoursesContextTests
    {
        private CoursesContext CreateContext(string dbName, out Mock<IMediator> mediatorMock)
        {
            mediatorMock = new Mock<IMediator>();
            var options = new DbContextOptionsBuilder<CoursesContext>()
                .UseInMemoryDatabase(dbName)
                .Options;

            return new CoursesContext(options, mediatorMock.Object);
        }

        [Fact]
        public async Task Commit_SetsCreatedDate_AndReturnsTrue_WhenSaveChangesSucceeds()
        {
            var db = CreateContext(Guid.NewGuid().ToString(), out var mediatorMock);

            var course = new Course { Name = "Ctx Course", Description = "D", Price = 1 };
            db.Courses.Add(course);

            var result = await db.Commit();

            Assert.True(result);
            Assert.NotEqual(default, course.CreatedDate);
        }

        [Fact]
        public async Task SaveChangesAsync_DoesNotModifyCreatedDate_OnUpdate()
        {
            var db = CreateContext(Guid.NewGuid().ToString(), out var mediatorMock);

            var course = new Course { Name = "Ctx Course 2", Description = "D", Price = 2 };
            db.Courses.Add(course);
            await db.SaveChangesAsync();

            var originalCreated = course.CreatedDate;

            course.Name = "Updated";
            await db.SaveChangesAsync();

            Assert.Equal(originalCreated, course.CreatedDate);
        }
    }
}
