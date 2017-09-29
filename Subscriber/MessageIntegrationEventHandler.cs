using System;
using System.Threading.Tasks;

namespace Subscriber
{
    public class MessageIntegrationEventHandler : EventBus.Abstractions.IIntegrationEventHandler<MessageIntegrationEvent>
    {
        public async Task Handle(MessageIntegrationEvent @event)
        {
            var message = @event.Message;
            var oppositeMessage = message.Description.Replace("Hello", "Goodbye");
            Console.WriteLine($" [x] {message.Type.ToString().ToLower()}: {oppositeMessage}");
        }
    }
}
