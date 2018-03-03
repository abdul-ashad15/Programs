using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Entity_MVC_Demo.Startup))]
namespace Entity_MVC_Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
