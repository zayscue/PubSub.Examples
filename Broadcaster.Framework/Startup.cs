using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Broadcaster.Framework.Startup))]

namespace Broadcaster.Framework
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}