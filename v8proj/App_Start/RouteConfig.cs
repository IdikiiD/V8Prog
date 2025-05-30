using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace v8proj
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");


            // 1. Добавляем маршрут для Админ-панели
            routes.MapRoute(
                name: "Admin",
                url: "Admin/{action}/{id}",
                defaults: new { controller = "Admin", action = "Dashboard", id = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "Auth",
                url: "Auth/{action}/{id}",
                defaults: new { controller = "Auth", action = "SignOut", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
                
            );
            routes.MapRoute(
                name: "Support",
                url: "Support/{action}/{id}",
                defaults: new { controller = "Support", action = "Support", id = UrlParameter.Optional }
            );

        }
    }
}