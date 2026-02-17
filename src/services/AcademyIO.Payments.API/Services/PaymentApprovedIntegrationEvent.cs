using System;
using AcademyIO.Core.Messages;

namespace AcademyIO.Core.Messages.Integration
{
    public class PaymentApprovedIntegrationEvent : IntegrationEvent
    {
        public Guid CourseId { get; private set; }
        public Guid StudentId { get; private set; }

        public PaymentApprovedIntegrationEvent(Guid courseId, Guid studentId)
        {
            CourseId = courseId;
            StudentId = studentId;
        }
    }
}