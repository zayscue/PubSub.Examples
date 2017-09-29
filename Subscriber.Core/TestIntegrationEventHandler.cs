using System;
using System.Threading.Tasks;

namespace Subscriber.Core
{
    public class TestIntegrationEventHandler : EventBus.Abstractions.IIntegrationEventHandler<TestIntegrationEvent>
    {
        public async Task Handle(TestIntegrationEvent @event)
        {
            var integer = @event.Test.TestInteger;
            Console.WriteLine($" [x] Random Number: {integer}");
        }
    }
}
