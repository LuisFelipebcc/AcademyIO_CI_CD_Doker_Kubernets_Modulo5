namespace AcademyIO.Bff.Extensions
{
    /// <summary>
    /// Represents the settings for the application services.
    /// </summary>
    public class AppServicesSettings
    {
        /// <summary>
        /// Gets or sets the URL for the authentication service.
        /// </summary>
        public string AuthUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL for the course service.
        /// </summary>
        public string CourseUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL for the student service.
        /// </summary>
        public string StudentUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL for the payment service.
        /// </summary>
        public string PaymentUrl { get; set; }
    }
}
