using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebServiceDemoApp.Startup))]
namespace WebServiceDemoApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
