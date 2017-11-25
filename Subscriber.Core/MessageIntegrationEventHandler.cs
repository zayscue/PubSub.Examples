using System;
using System.Threading.Tasks;

namespace Subscriber.Core
{
    public class MessageIntegrationEventHandler : EventBus.Abstractions.IIntegrationEventHandler<MessageIntegrationEvent>
    {
        public Task Handle(MessageIntegrationEvent @event)
        {
            var message = @event.Message;
            Console.WriteLine($" [x] {message.Type.ToString().ToLower()}: {message.Description}");
            return Task.FromResult(0);
        }
    }
}
