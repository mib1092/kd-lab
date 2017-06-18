using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IMCMS.Web
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapMvcAttributeRoutes();
            routes.LowercaseUrls = true;

            routes.MapRoute(
                name: "imageuploader",
                url: "imageuploader/{action}/{id}",
                defaults: new { controller = "ImageUploader", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "IMCMS.Web.Controllers" }
            );


        }
    }
}