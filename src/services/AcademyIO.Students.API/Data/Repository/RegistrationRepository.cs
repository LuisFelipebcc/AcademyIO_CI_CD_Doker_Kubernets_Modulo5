using AcademyIO.Core.Data;
using AcademyIO.Core.Enums;
using AcademyIO.Students.API.Models;
using Microsoft.EntityFrameworkCore;

namespace AcademyIO.Students.API.Data.Repository
{
    public class RegistrationRepository(StudentsContext db) : IRegistrationRepository
    {
        private readonly DbSet<Registration> _dbSet = db.Set<Registration>();
        private readonly DbSet<Certification> _dbSetCertification = db.Set<Certification>();
        public IUnitOfWork UnitOfWork => db;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Marks a registration as completed and creates a certification for the student
        /// </summary>
        /// <param name="studentId">ID of the student</param>
        /// <param name="courseId">ID of the course</param>
        /// <returns>The updated registration with completed status</returns>
        public async Task<Registration> FinishCourse(Guid studentId, Guid courseId)
        {
            var registration = await _dbSet.FirstOrDefaultAsync(a => a.StudentId == studentId && a.CourseId == courseId);

            if (registration == null)
                throw new InvalidOperationException($"No registration found for student {studentId} in course {courseId}");

            // Mark registration as completed
            registration.Status = EProgressLesson.Completed;

            // Create certification for the student
            var certification = new Certification(courseId, studentId);
            _dbSetCertification.Add(certification);

            return registration;
        }

        public Registration AddRegistration(Guid studentId, Guid courseId)
        {
            if (RegistrationExists(studentId, courseId))
                throw new Exception("Matrícula já existente.");

            var registration = new Registration(studentId, courseId, DateTime.Now);


            _dbSet.Add(registration);

            return registration;
        }

        private bool RegistrationExists(Guid studentId, Guid courseId)
        {
            return _dbSet.Any(x => x.StudentId == studentId && x.CourseId == courseId);
        }

        public List<Registration> GetRegistrationByStudent(Guid studentId)
        {
            return _dbSet.Where(x => x.StudentId == studentId).ToList();
        }

        public List<Registration> GetAllRegistrations()
        {
            return _dbSet.ToList();
        }
    }
}
