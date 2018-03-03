using System.Web.Mvc;

namespace XEPR.Areas.Administrator
{
    public class AdministratorAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Administrator";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Administrator_default",
                "Administrator/{controller}/{action}/{id}",
                new {Controller ="Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
