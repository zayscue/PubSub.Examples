using System;

namespace Subscriber.Core
{
    public class TestIntegrationEvent : EventBus.Events.IntegrationEvent
    {
        public Test Test { get; set; }

        public TestIntegrationEvent(Test test) => Test = test;
    }

    public class Test
    {
        public int TestInteger { get; set; }

        public Test()
        {
            var rnd = new Random();
            TestInteger = rnd.Next(1, 1000);
        }
    }
}
