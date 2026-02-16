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
    public class CourseRepositoryTests
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
        public async Task Add_GetAll_GetById_Update_Delete_Workflow()
        {
            var db = CreateContext(Guid.NewGuid().ToString());
            var repo = new CourseRepository(db);

            var course = new Course { Id = Guid.NewGuid(), Name = "Course A", Description = "Desc", Price = 50 };
            repo.Add(course);
            await db.Commit();

            var all = (await repo.GetAll()).ToList();
            Assert.Single(all);

            var fetched = await repo.GetById(course.Id);
            Assert.NotNull(fetched);
            Assert.Equal("Course A", fetched.Name);
            Assert.True(repo.CourseExists(course.Id));

            // Update
            fetched.Name = "Updated";
            repo.Update(fetched);
            await db.Commit();

            var updated = await repo.GetById(course.Id);
            Assert.Equal("Updated", updated.Name);

            // Delete
            repo.Delete(updated);
            await db.Commit();

            Assert.False(repo.CourseExists(course.Id));
        }
    }
}
