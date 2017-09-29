using System.Threading.Tasks;
using Broadcaster.Framework.Hubs;
using EventBus.Abstractions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;

namespace Broadcaster.Framework
{
    public class MessageIntegrationEventHandler<T> : IIntegrationEventHandler<MessageIntegrationEvent> where T : IHub
    {
        private readonly HubConfiguration _hubConfig;

        private IHubConnectionContext<dynamic> Clients
            => _hubConfig.Resolver.Resolve<IConnectionManager>().GetHubContext<T>().Clients;

        public MessageIntegrationEventHandler(HubConfiguration config)
        {
            _hubConfig = config;
        }

        public async Task Handle(MessageIntegrationEvent @event)
        {
            Clients.All.getMessage(@event.Message);
        }
    }
}