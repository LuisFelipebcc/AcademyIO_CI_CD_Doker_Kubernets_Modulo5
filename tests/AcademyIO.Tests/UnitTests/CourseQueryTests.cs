using AcademyIO.Courses.API.Application.Queries;
using AcademyIO.Courses.API.Application.Queries.ViewModels;
using AcademyIO.Courses.API.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class CourseQueryTests
    {
        private readonly Mock<ICourseRepository> _courseRepoMock;
        private readonly CourseQuery _query;

        public CourseQueryTests()
        {
            _courseRepoMock = new Mock<ICourseRepository>();
            _query = new CourseQuery(_courseRepoMock.Object);
        }

        [Fact]
        public async Task GetAll_WithCourses_ReturnsViewModels()
        {
            var courses = new List<Course>
            {
                new Course { Id = Guid.NewGuid(), Name = "Course 1", Description = "Desc 1", Price = 100 },
                new Course { Id = Guid.NewGuid(), Name = "Course 2", Description = "Desc 2", Price = 200 }
            };

            _courseRepoMock.Setup(r => r.GetAll()).ReturnsAsync(courses);

            var result = await _query.GetAll();

            var resultList = new List<CourseViewModel>(result);
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Course 1", resultList[0].Name);
            Assert.Equal("Course 2", resultList[1].Name);
        }

        [Fact]
        public async Task GetAll_WithNoCourses_ReturnsEmptyList()
        {
            _courseRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Course>());

            var result = await _query.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetById_WithExistingCourse_ReturnsViewModel()
        {
            var courseId = Guid.NewGuid();
            var course = new Course { Id = courseId, Name = "Course 1", Description = "Desc 1", Price = 100 };

            _courseRepoMock.Setup(r => r.CourseExists(courseId)).Returns(true);
            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);

            var result = await _query.GetById(courseId);

            Assert.NotNull(result);
            Assert.Equal("Course 1", result.Name);
            Assert.Equal(100, result.Price);
        }

        [Fact]
        public async Task GetById_WithNonExistingCourse_ReturnsNull()
        {
            var courseId = Guid.NewGuid();

            _courseRepoMock.Setup(r => r.CourseExists(courseId)).Returns(false);

            var result = await _query.GetById(courseId);

            Assert.Null(result);
            _courseRepoMock.Verify(r => r.GetById(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetById_MapsCoursePropertiesToViewModel()
        {
            var courseId = Guid.NewGuid();
            var course = new Course
            {
                Id = courseId,
                Name = "Advanced C#",
                Description = "Learn advanced C# concepts",
                Price = 299.99
            };

            _courseRepoMock.Setup(r => r.CourseExists(courseId)).Returns(true);
            _courseRepoMock.Setup(r => r.GetById(courseId)).ReturnsAsync(course);

            var result = await _query.GetById(courseId);

            Assert.Equal(courseId, result.Id);
            Assert.Equal("Advanced C#", result.Name);
            Assert.Equal("Learn advanced C# concepts", result.Description);
            Assert.Equal(299.99, result.Price);
        }
    }
}
