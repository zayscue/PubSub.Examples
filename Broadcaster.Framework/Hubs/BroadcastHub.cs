using Autofac;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Broadcaster.Framework.Hubs
{
    [HubName("broadcaster")]
    public class BroadcastHub : Hub
    {
        private readonly ILifetimeScope _hubLifetimeScope;

        public BroadcastHub(ILifetimeScope lifetimeScope)
        {
            _hubLifetimeScope = lifetimeScope.BeginLifetimeScope();
        }
    }
}