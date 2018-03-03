using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JsonDemo.Startup))]
namespace JsonDemo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
