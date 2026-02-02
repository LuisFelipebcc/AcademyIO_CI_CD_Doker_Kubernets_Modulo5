using AcademyIO.Core.DomainObjects;

namespace AcademyIO.Students.API.Models
{
    /// <summary>
    /// Represents a student's course certification upon completion
    /// </summary>
    public class Certification : Entity
    {
        public Guid CourseId { get; set; }
        public Guid StudentId { get; set; }
        public StudentUser Student { get; private set; }

        /// <summary>
        /// Date when the certification was awarded
        /// </summary>
        public DateTime CertificationDate { get; set; }

        /// <summary>
        /// Certification code for verification purposes
        /// </summary>
        public string CertificationCode { get; set; }

        public Certification()
        {
        }

        public Certification(Guid courseId, Guid studentId)
        {
            CourseId = courseId;
            StudentId = studentId;
            CertificationDate = DateTime.Now;
            CertificationCode = GenerateCertificationCode();
        }

        private string GenerateCertificationCode()
        {
            return $"CERT-{StudentId:N}-{CourseId:N}-{DateTime.UtcNow.Ticks}";
        }
    }
}
