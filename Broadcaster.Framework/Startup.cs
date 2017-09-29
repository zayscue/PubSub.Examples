using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Integration.SignalR;
using Broadcaster.Framework.Hubs;
using EventBus.Abstractions;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Broadcaster.Framework.Startup))]

namespace Broadcaster.Framework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            var config = new HubConfiguration();
            builder.RegisterHubs(Assembly.GetExecutingAssembly());
            builder.RegisterInstance(config);
            builder.Register(x => new EventBusRabbitMQ.EventBusRabbitMQ("s99-rabbitmq.ecsdev.com")).As<IEventBus>().SingleInstance();
            builder.RegisterType<MessageIntegrationEventHandler<BroadcastHub>>().InstancePerDependency();
            builder.RegisterBuildCallback(c =>
            {
                var eventbus = c.Resolve<IEventBus>();
                var handler = c.Resolve<MessageIntegrationEventHandler<BroadcastHub>>();
                eventbus.Subscribe(handler);
            });

            var container = builder.Build();
            config.Resolver = new AutofacDependencyResolver(container);

            app.UseAutofacMiddleware(container);

            app.MapSignalR("/signalr", config);
        }
    }
}