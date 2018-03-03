using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCDemoApp.Startup))]
namespace MVCDemoApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
