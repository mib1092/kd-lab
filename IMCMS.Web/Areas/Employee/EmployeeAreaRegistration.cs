using System.Web.Mvc;

namespace IMCMS.Web.Areas.Employee
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
                name: "Employee_login",
                url: "EmployeePortal/Account/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional },
                namespaces: new[] { "IMCMS.Web.Areas.Employee.Controllers" }
            );

            context.MapRoute(
                name: "Employee_default",
                url: "EmployeePortal/{*slug}",
                defaults: new { controller = "Home", action = "Index", slug = UrlParameter.Optional },
                namespaces: new[] { "IMCMS.Web.Areas.Employee.Controllers" }
            );

            /*
            context.MapRoute(
                name: "Employee_default",
                url: "EmployeePortal/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", rootpage = UrlParameter.Optional },
                namespaces: new[] { "IMCMS.Web.Areas.Employee.Controllers" }
            );
            */
        }
    }
}