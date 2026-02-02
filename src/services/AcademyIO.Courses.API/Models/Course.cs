using AcademyIO.Core.DomainObjects;

namespace AcademyIO.Courses.API.Models
{
    public class Course : Entity, IAggregateRoot
    {
        public Course() : base()
        {
            _lessons = new List<Lesson>();
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }

        private readonly List<Lesson> _lessons;
        public IReadOnlyCollection<Lesson> Lessons => _lessons;

        public void AddLesson(Lesson lesson)
        {
            if (lesson == null)
                throw new ArgumentNullException(nameof(lesson));

            if (LessonExists(lesson))
                throw new InvalidOperationException($"Lesson '{lesson.Name}' already exists in this course");

            lesson.CourseId = this.Id;
            _lessons.Add(lesson);
        }

        private bool LessonExists(Lesson lesson)
        {
            return _lessons.Any(l => l.Name == lesson.Name && !l.Deleted);
        }
    }
}
