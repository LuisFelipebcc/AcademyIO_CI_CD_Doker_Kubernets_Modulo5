using AcademyIO.Bff.Services;
using AcademyIO.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyIO.Bff.Controllers
{
    /// <summary>
    /// Represents the students controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : MainController
    {
        private readonly IStudentService _studentService;
        private readonly ICourseService _courseService;
        private readonly IPaymentService _paymentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentsController"/> class.
        /// </summary>
        /// <param name="studentService">The student service.</param>
        /// <param name="courseService">The course service.</param>
        /// <param name="paymentService">The payment service.</param>
        public StudentsController(
        IStudentService studentService,
        ICourseService courseService,
        IPaymentService paymentService)
        {
            _studentService = studentService;
            _courseService = courseService;
            _paymentService = paymentService;
        }

        /// <summary>
        /// Registers a student to a course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("register-to-course")]
        public async Task<IActionResult> RegisterToCourse(Guid courseId)
        {
            var course = await _courseService.GetById(courseId);
            if (course == null)
                return NotFound("Curso não encontrado.");

            var paymentExists = await _paymentService.PaymentExists(courseId);
            if (!paymentExists)
                return UnprocessableEntity("Você não possui acesso a esse curso.");

            var response = await _studentService.RegisterToCourse(courseId);

            return CustomResponse(response);
        }

        /// <summary>
        /// Gets the registration for the current student.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpGet]
        [Route("get-registration")]
        public async Task<IActionResult> GetRegistration()
        {
            var response = await _studentService.GetRegistration();
            return CustomResponse(response);
        }

        /// <summary>
        /// Gets all registrations.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        [Route("get-all-registrations")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _studentService.GetAllRegistrations();
            return CustomResponse(response);
        }
    }
}
