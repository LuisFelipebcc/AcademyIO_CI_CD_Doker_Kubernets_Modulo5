namespace AcademyIO.Payments.API.Services
{
    public class PaymentInputModel
    {
        public Guid CourseId { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string CardExpirationDate { get; set; }
        public string CardCVV { get; set; }
    }
}