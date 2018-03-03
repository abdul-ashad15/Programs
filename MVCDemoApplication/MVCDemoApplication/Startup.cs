using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVCDemoApplication.Startup))]
namespace MVCDemoApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
