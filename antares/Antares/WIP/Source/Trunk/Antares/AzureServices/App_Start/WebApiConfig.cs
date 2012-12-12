using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AzureServices
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, 
                                username = RouteParameter.Optional, 
                                projectid = RouteParameter.Optional, 
                                userid = RouteParameter.Optional,
                                role = RouteParameter.Optional, }
            );
        }
    }
}
