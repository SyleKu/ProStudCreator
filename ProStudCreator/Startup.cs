using Microsoft.Owin;
using Owin;
using ProStudCreator;

[assembly: OwinStartup(typeof(Startup))]

namespace ProStudCreator
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}