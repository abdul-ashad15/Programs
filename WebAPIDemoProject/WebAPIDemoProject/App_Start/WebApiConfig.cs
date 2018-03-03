using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Cors;
namespace WebAPIDemoProject
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            var cors = new EnableCorsAttribute("*", "*", "*");
            config.EnableCors(cors);

            config.Routes.MapHttpRoute(
            name: "Employees",
            routeTemplate: "api/Students/{id}",
            defaults: new { controller = "Students", action = "GetStudents", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { controller = "Students", action = "Get", id = RouteParameter.Optional }
            );

            //config.Formatters.Remove(config.Formatters.XmlFormatter); //this will remove xml formatter and always return in Json formatter
            //config.Formatters.Remove(config.Formatters.JsonFormatter);//this will remove json formatter and always return in xml formatter
        }
    }
}
