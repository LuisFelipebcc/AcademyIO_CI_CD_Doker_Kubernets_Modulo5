using AcademyIO.Courses.API.Models;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace AcademyIO.Courses.API.Tests.Models;

public class CourseTests
{
    [Fact]
    public void Course_Constructor_ShouldInitializeLessonsCollection()
    {
        // Act
        var course = new Course();

        // Assert
        course.Lessons.Should().NotBeNull();
        course.Lessons.Should().BeEmpty();
    }

    [Fact]
    public void AddLesson_WithValidLesson_ShouldAddToCollection()
    {
        // Arrange
        var course = new Course { Name = "C# Basics" };
        var lesson = new Lesson("Introduction", "Fundamentals", 2.0, course.Id);

        // Act
        course.AddLesson(lesson);

        // Assert
        course.Lessons.Should().HaveCount(1);
        course.Lessons.First().Should().Be(lesson);
    }

    [Fact]
    public void AddLesson_WithNullLesson_ShouldThrowArgumentNullException()
    {
        // Arrange
        var course = new Course();

        // Act & Assert
        var ex = Assert.Throws<ArgumentNullException>(() => course.AddLesson(null!));
        ex.ParamName.Should().Be("lesson");
    }

    [Fact]
    public void AddLesson_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Name = "C# Basics", Id = courseId };
        var lesson1 = new Lesson("Introduction", "Fundamentals", 2.0, courseId);
        var lesson2 = new Lesson("Introduction", "Advanced", 3.0, courseId);

        course.AddLesson(lesson1);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => course.AddLesson(lesson2));
        ex.Message.Should().Contain("already exists");
    }

    [Fact]
    public void AddLesson_MultipleUniqueLessons_ShouldAddAll()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Name = "C# Advanced", Id = courseId };
        var lesson1 = new Lesson("Generics", "Advanced", 2.0, courseId);
        var lesson2 = new Lesson("LINQ", "Advanced", 3.0, courseId);
        var lesson3 = new Lesson("Async/Await", "Advanced", 2.5, courseId);

        // Act
        course.AddLesson(lesson1);
        course.AddLesson(lesson2);
        course.AddLesson(lesson3);

        // Assert
        course.Lessons.Should().HaveCount(3);
        course.Lessons.Should().Contain(new[] { lesson1, lesson2, lesson3 });
    }

    [Fact]
    public void AddLesson_ShouldSetCourseIdOnLesson()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Name = "C# Basics", Id = courseId };
        var lesson = new Lesson("Introduction", "Fundamentals", 2.0, Guid.Empty);

        // Act
        course.AddLesson(lesson);

        // Assert
        lesson.CourseId.Should().Be(courseId);
    }

    [Fact]
    public void AddLesson_DeletedLesson_ShouldNotBeConsidered()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var course = new Course { Name = "C# Basics", Id = courseId };
        var lesson1 = new Lesson("Introduction", "Fundamentals", 2.0, courseId);
        var lesson2 = new Lesson("Introduction", "Advanced", 3.0, courseId);

        course.AddLesson(lesson1);
        lesson1.Deleted = true;

        // Act & Assert
        // Should not throw because deleted lesson should not be considered a duplicate
        course.AddLesson(lesson2);
        course.Lessons.Should().HaveCount(2);
    }

    [Fact]
    public void Course_LessonsProperty_ShouldBeReadOnly()
    {
        // Arrange
        var course = new Course();

        // Assert
        var lessonsProperty = typeof(Course).GetProperty("Lessons");
        var setMethod = lessonsProperty?.GetSetMethod();

        setMethod.Should().BeNull("Lessons property should be read-only");
    }
}
