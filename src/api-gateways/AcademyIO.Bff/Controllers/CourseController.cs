using AcademyIO.Bff.Models;
using AcademyIO.Bff.Services;
using AcademyIO.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcademyIO.Bff.Controllers
{
    /// <summary>
    /// Represents the course controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : MainController
    {
        private readonly ICourseService _courseService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseController"/> class.
        /// </summary>
        /// <param name="courseService">The course service.</param>
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        /// <summary>
        /// Gets all courses.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("courses")]
        public async Task<IActionResult> GetAll()
        {
            var response = await _courseService.GetAll();
            return CustomResponse(response);
        }

        /// <summary>
        /// Gets a course by its identifier.
        /// </summary>
        /// <param name="id">The course identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet]
        [Route("course")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _courseService.GetById(id);
            return CustomResponse(response);
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="course">The course view model.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("create-course")]
        public async Task<IActionResult> CreateCourse(CourseViewModel course)
        {
            var response = await _courseService.Create(course);
            return CustomResponse(response);
        }

        /// <summary>
        /// Updates an existing course.
        /// </summary>
        /// <param name="course">The course view model.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPut]
        [Route("update-course")]
        public async Task<IActionResult> UpdateCourse(CourseViewModel course)
        {
            var response = await _courseService.Update(course);
            return CustomResponse(response);
        }

        /// <summary>
        /// Removes a course by its identifier.
        /// </summary>
        /// <param name="id">The course identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpDelete]
        [Route("remove-course")]
        public async Task<IActionResult> RemoveCourse(Guid id)
        {
            var response = await _courseService.Remove(id);
            return CustomResponse(response);
        }

        /// <summary>
        /// Makes a payment for a course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <param name="payment">The payment view model.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("make-payment")]
        public async Task<IActionResult> MakePaymentCourse(Guid courseId, PaymentViewModel payment)
        {
            var response = await _courseService.MakePayment(courseId, payment);
            return CustomResponse(response);
        }

        /// <summary>
        /// Gets the lessons for a course.
        /// </summary>
        /// <param name="courseId">The course identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet]
        [Route("lesson-course")]
        public async Task<IActionResult> GetLessonByCourse(Guid courseId)
        {
            var response = await _courseService.GetLessonByCourse(courseId);
            return CustomResponse(response);
        }

        /// <summary>
        /// Gets the progress of a lesson.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpGet]
        [Route("progress-lesson")]
        public async Task<IActionResult> GetProgressLesson()
        {
            var response = await _courseService.GetProgressLesson();
            return CustomResponse(response);
        }

        /// <summary>
        /// Creates a new lesson.
        /// </summary>
        /// <param name="lesson">The lesson view model.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("create-lesson")]
        public async Task<IActionResult> CreateLesson(LessonViewModel lesson)
        {
            var response = await _courseService.CreateLesson(lesson);
            return CustomResponse(response);
        }

        /// <summary>
        /// Starts a lesson.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("start-lesson")]
        public async Task<IActionResult> StartLesson(Guid lessonId)
        {
            var response = await _courseService.StartLesson(lessonId);
            return CustomResponse(response);
        }

        /// <summary>
        /// Finishes a lesson.
        /// </summary>
        /// <param name="lessonId">The lesson identifier.</param>
        /// <returns>An <see cref="IActionResult"/> representing the result of the operation.</returns>
        [HttpPost]
        [Route("finish-lesson")]
        public async Task<IActionResult> FinishLesson(Guid lessonId)
        {
            var response = await _courseService.FinishLesson(lessonId);
            return CustomResponse(response);
        }
    }
}
