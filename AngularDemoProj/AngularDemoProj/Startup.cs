using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AngularDemoProj.Startup))]
namespace AngularDemoProj
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
