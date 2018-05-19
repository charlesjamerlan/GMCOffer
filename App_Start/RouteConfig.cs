using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace GMCOffer
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ThankYou",
                url: "thankyou/{action}/{id}",
                defaults: new { controller = "Content", action = "ThankYou", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Get",
                url: "get/{action}/{id}",
                defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Limit",
                url: "limit/{action}/{id}",
                defaults: new { controller = "Content", action = "Limit", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Error",
                url: "error/{action}/{id}",
                defaults: new { controller = "Content", action = "Error", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Eligibility",
                url: "eligibility/{action}/{id}",
                defaults: new { controller = "Content", action = "Eligibility", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Expired",
                url: "expired/{action}/{id}",
                defaults: new { controller = "Content", action = "Expired", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Content", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}