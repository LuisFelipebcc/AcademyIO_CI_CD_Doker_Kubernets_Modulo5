using AcademyIO.Core.DomainObjects;
using AcademyIO.Core.Messages;
using System;
using Xunit;

namespace AcademyIO.Tests.UnitTests
{
    public class EntityTests
    {
        private class TestEntity : Entity
        {
            public TestEntity() : base() { }
            public TestEntity(Guid id) : base(id) { }
        }

        [Fact]
        public void AddRemoveCleanEvents_WorkAsExpected()
        {
            var e = new TestEntity();
            var ev = new TestEvent();

            Assert.Empty(e.Notifications);
            e.AddEvent(ev);
            Assert.Single(e.Notifications);
            e.RemoveEvent(ev);
            Assert.Empty(e.Notifications);

            e.AddEvent(ev);
            Assert.Single(e.Notifications);
            e.CleanEvents();
            Assert.Empty(e.Notifications);
        }

        [Fact]
        public void EqualsAndOperators_WorkAsExpected()
        {
            var a = new TestEntity();
            var b = new TestEntity(a.Id);
            var c = new TestEntity();

            Assert.True(a == b);
            Assert.False(a != b);
            Assert.False(a == c);
            Assert.True(a != c);
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
            Assert.Contains(a.GetType().Name, a.ToString());
        }

        private class TestEvent : Event { }
    }
}
