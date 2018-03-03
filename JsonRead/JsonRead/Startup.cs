using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JsonRead.Startup))]
namespace JsonRead
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
