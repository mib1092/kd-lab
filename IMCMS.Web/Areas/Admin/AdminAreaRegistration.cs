using System.Web.Mvc;

namespace IMCMS.Web.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {

            context.MapRoute(
                "Account",
                "SiteAdmin/Account/{action}/{id}",
                new { controller = "Account", action = "Index", id = UrlParameter.Optional },
                new[] { "IMCMS.Common.Controllers" }
                );

            context.MapRoute(
                "Admin_default",
                "SiteAdmin/{controller}/{action}/{id}",
                new { controller = "Default", action = "Index", id = UrlParameter.Optional },
                new[] { "IMCMS.Web.Areas.Admin.Controllers" }
            );
        }
    }
}
