namespace AcademyIO.Bff.Models
{
    /// <summary>
    /// Represents the course view model.
    /// </summary>
    public class CourseViewModel
    {
        /// <summary>
        /// Gets or sets the course identifier.
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Gets or sets the course name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the course description.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Gets or sets the course price.
        /// </summary>
        public double Price { get; set; }
    }
}
