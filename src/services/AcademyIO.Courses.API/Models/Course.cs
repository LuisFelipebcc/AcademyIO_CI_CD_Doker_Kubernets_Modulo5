using AcademyIO.Core.DomainObjects;

namespace AcademyIO.Courses.API.Models
{
    public class Course : Entity, IAggregateRoot
    {
        public Course() : base()
        {
            _lessons = new List<Lesson>();
            Name = string.Empty;
            Description = string.Empty;
        }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }

        private readonly List<Lesson> _lessons = new List<Lesson>();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        public void AddLesson(Lesson lesson)
        {
            if (lesson is null) throw new ArgumentNullException(nameof(lesson));
            if (LessonExistis(lesson)) throw new InvalidOperationException("Lesson already exists.");
            _lessons.Add(lesson);
        }

        private bool LessonExistis(Lesson lesson)
        {
            return _lessons.Any(l => l.Id == lesson.Id);
        }
    }
}
