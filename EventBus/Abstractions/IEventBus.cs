using EventBus.Events;

namespace EventBus.Abstractions
{
    public interface IEventBus
    {
        void Publish(IntegrationEvent @event);
        void Subscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent;
        void Unsubscribe<T>(IIntegrationEventHandler<T> handler) where T : IntegrationEvent;
    }
}