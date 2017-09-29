using EventBus.Events;
using Models;

namespace Broadcaster.Framework
{
    public class MessageIntegrationEvent : IntegrationEvent
    {
        public Message Message { get; set; }

        public MessageIntegrationEvent(Message message)
        {
            Message = message;
        }
    }
}