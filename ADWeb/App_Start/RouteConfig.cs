using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ADWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            
            routes.MapRoute(
                name: "UsersSection",
                url: "Users/{action}/{userId}",
                defaults: new { controller = "Users", action = "Index", userId = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "HomeSection",
                url: "{action}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
