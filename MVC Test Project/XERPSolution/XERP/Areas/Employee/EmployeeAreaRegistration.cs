using System.Web.Mvc;

namespace XEPR.Areas.Employee
{
    public class EmployeeAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Employee";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Employee_default",
                "Employee/{controller}/{action}/{id}",
                new {Controller ="Employee", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
