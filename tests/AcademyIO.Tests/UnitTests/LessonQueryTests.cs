using AcademyIO.Core.Enums;
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
    public class LessonQueryTests
    {
        private readonly Mock<ILessonRepository> _lessonRepoMock;
        private readonly LessonQuery _query;

        public LessonQueryTests()
        {
            _lessonRepoMock = new Mock<ILessonRepository>();
            _query = new LessonQuery(_lessonRepoMock.Object);
        }

        [Fact]
        public async Task GetAll_WithLessons_ReturnsViewModels()
        {
            var courseId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                new Lesson("Lesson 1", "Subject 1", 10, courseId) { Id = Guid.NewGuid() },
                new Lesson("Lesson 2", "Subject 2", 15, courseId) { Id = Guid.NewGuid() }
            };

            _lessonRepoMock.Setup(r => r.GetAll()).ReturnsAsync(lessons);

            var result = await _query.GetAll();

            var resultList = new List<LessonViewModel>(result);
            Assert.Equal(2, resultList.Count);
            Assert.Equal("Lesson 1", resultList[0].Name);
            Assert.Equal("Lesson 2", resultList[1].Name);
        }

        [Fact]
        public async Task GetAll_WithNoLessons_ReturnsEmptyList()
        {
            _lessonRepoMock.Setup(r => r.GetAll()).ReturnsAsync(new List<Lesson>());

            var result = await _query.GetAll();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByCourseId_WithLessonsInCourse_ReturnsViewModels()
        {
            var courseId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                new Lesson("Lesson 1", "Subject 1", 10, courseId) { Id = Guid.NewGuid() },
                new Lesson("Lesson 2", "Subject 2", 15, courseId) { Id = Guid.NewGuid() }
            };

            _lessonRepoMock.Setup(r => r.GetByCourseId(courseId)).ReturnsAsync(lessons);

            var result = await _query.GetByCourseId(courseId);

            var resultList = new List<LessonViewModel>(result);
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, l => Assert.Equal(courseId, l.CourseId));
        }

        [Fact]
        public async Task GetByCourseId_WithNodeLessons_ReturnsEmptyList()
        {
            var courseId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.GetByCourseId(courseId)).ReturnsAsync(new List<Lesson>());

            var result = await _query.GetByCourseId(courseId);

            Assert.Empty(result);
        }

        [Fact]
        public void ExistsProgress_WithExistingProgress_ReturnsTrue()
        {
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.ExistProgress(lessonId, studentId)).Returns(true);

            var result = _query.ExistsProgress(lessonId, studentId);

            Assert.True(result);
        }

        [Fact]
        public void ExistsProgress_WithNonExistingProgress_ReturnsFalse()
        {
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.ExistProgress(lessonId, studentId)).Returns(false);

            var result = _query.ExistsProgress(lessonId, studentId);

            Assert.False(result);
        }

        [Fact]
        public void GetProgressStatusLesson_ReturnsProgressStatus()
        {
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.GetProgressStatusLesson(lessonId, studentId))
                .Returns(EProgressLesson.Finished);

            var result = _query.GetProgressStatusLesson(lessonId, studentId);

            Assert.Equal(EProgressLesson.Finished, result);
        }

        [Fact]
        public void GetProgressStatusLesson_Returns_NotStarted()
        {
            var lessonId = Guid.NewGuid();
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.GetProgressStatusLesson(lessonId, studentId))
                .Returns(EProgressLesson.NotStarted);

            var result = _query.GetProgressStatusLesson(lessonId, studentId);

            Assert.Equal(EProgressLesson.NotStarted, result);
        }

        [Fact]
        public async Task GetProgress_WithStudentProgressions_ReturnsViewModels()
        {
            var studentId = Guid.NewGuid();
            var progressions = new List<ProgressLesson>
            {
                new ProgressLesson
                {
                    Id = Guid.NewGuid(),
                    Lesson = new Lesson("Lesson 1", "Subject 1", 10, Guid.NewGuid()) { Id = Guid.NewGuid() },
                    ProgressionStatus = EProgressLesson.InProgress
                },
                new ProgressLesson
                {
                    Id = Guid.NewGuid(),
                    Lesson = new Lesson("Lesson 2", "Subject 2", 15, Guid.NewGuid()) { Id = Guid.NewGuid() },
                    ProgressionStatus = EProgressLesson.Finished
                }
            };

            _lessonRepoMock.Setup(r => r.GetProgression(studentId)).ReturnsAsync(progressions);

            var result = await _query.GetProgress(studentId);

            var resultList = new List<LessonProgressViewModel>(result);
            Assert.Equal(2, resultList.Count);
        }

        [Fact]
        public async Task GetProgress_WithNoProgressions_ReturnsEmptyList()
        {
            var studentId = Guid.NewGuid();

            _lessonRepoMock.Setup(r => r.GetProgression(studentId)).ReturnsAsync(new List<ProgressLesson>());

            var result = await _query.GetProgress(studentId);

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAll_MapsLessonPropertiesToViewModel()
        {
            var lessonId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var lessons = new List<Lesson>
            {
                new Lesson("Advanced Lesson", "Advanced Subject", 25.5, courseId) { Id = lessonId }
            };

            _lessonRepoMock.Setup(r => r.GetAll()).ReturnsAsync(lessons);

            var result = await _query.GetAll();

            var resultList = new List<LessonViewModel>(result);
            var lesson = resultList[0];

            Assert.Equal(lessonId, lesson.Id);
            Assert.Equal("Advanced Lesson", lesson.Name);
            Assert.Equal("Advanced Subject", lesson.Subject);
            Assert.Equal(25.5, lesson.TotalHours);
            Assert.Equal(courseId, lesson.CourseId);
        }
    }
}
