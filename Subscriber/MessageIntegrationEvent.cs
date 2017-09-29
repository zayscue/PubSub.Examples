namespace Subscriber
{
    public class MessageIntegrationEvent : EventBus.Events.IntegrationEvent
    {
        public Models.Message Message { get; set; }

        public MessageIntegrationEvent(Models.Message message)
        {
            Message = message;
        }
    }
}
