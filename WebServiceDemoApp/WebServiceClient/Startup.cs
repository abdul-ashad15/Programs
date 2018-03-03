using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebServiceClient.Startup))]
namespace WebServiceClient
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
