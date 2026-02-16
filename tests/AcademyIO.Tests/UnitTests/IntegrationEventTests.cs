using AcademyIO.Core.Messages.Integration;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class IntegrationEventTests
    {
        [Fact]
        public void UserRegisteredIntegrationEvent_SetsProperties()
        {
            var id = Guid.NewGuid();
            var ev = new UserRegisteredIntegrationEvent(id, "A", "B", "a@b.com", DateTime.UtcNow, false);
            Assert.Equal("UserRegisteredIntegrationEvent", ev.MessageType);
            Assert.Equal(id, ev.Id);
        }
    }
}
