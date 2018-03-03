using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CodeFirstDemo.Startup))]
namespace CodeFirstDemo
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
