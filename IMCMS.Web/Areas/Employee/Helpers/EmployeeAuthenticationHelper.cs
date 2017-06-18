using System;
using System.Configuration;
using System.Security.Principal;
using System.Web.Mvc;

namespace IMCMS.Web.Areas.Employee.Helpers
{
    public static class EmployeeAuthenticationHelper
    {
        public static void LoginUser(this Controller controller)
        {
            //Create session for user
            controller.Session["EmployeePortalUser"] = "employee";
            controller.Session.Timeout = Int32.Parse(ConfigurationManager.AppSettings["SessionPublicUsersTimeOut"] ?? "60");
        }

        public static void LogoutUser(this Controller controller)
        {
            //Logout user from session
            controller.Session["EmployeePortalUser"] = null;
            controller.Session.Clear();
        }

        public static bool IsUserAuthenticated(this Controller controller)
        {
            if (controller.User.Identity.IsAuthenticated) return true;
            else if (controller.Session["EmployeePortalUser"] == null) return false;
            else if (controller.Session["EmployeePortalUser"].ToString() == "employee") return true;
            return false;
        }
    }
}