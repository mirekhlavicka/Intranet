using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IntranetPublic
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Home",
                url: "", 
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "TimeTable",
                url: "timetable/{uid}",
                defaults: new { controller = "TimeTable", action = "Index", uid = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Downloads",
                url: "downloads",
                defaults: new { controller = "Downloads", action = "Index" }
            );

            routes.MapRoute(
                name: "Courses",
                url: "courses/{lang}",
                defaults: new { controller = "Courses", action = "Index" , lang = UrlParameter.Optional}
            );

            routes.MapRoute(
                name: "Staff",
                url: "staff/{lang}",
                defaults: new { controller = "Staff", action = "Index", lang = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "UserTimeTable",
                url: "{username}/rozvrh",
                defaults: new { controller = "UserHomePage", action = "TimeTable" }
            );

            routes.MapRoute(
                name: "UserDownloads",
                url: "{username}/soubory-ke-stazeni",
                defaults: new { controller = "UserHomePage", action = "Downloads" }
            );

            routes.MapRoute(
                name: "UserHomePage",
                url: "{username}/{page}",
                defaults: new { controller = "UserHomePage", action = "Index", page = UrlParameter.Optional }               
            );

        }
    }
}
