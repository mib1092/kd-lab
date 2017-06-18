using IMCMS.Common;
using IMCMS.Common.Controllers;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    [AuthorizeRoles(Constants.ROLE_USERS_IM)]
    public class DeveloperController : BaseAdminController
    {
        public DeveloperController(IUnitOfWork uow) : base(uow)
        {

        }

        public ActionResult RecycleApplication()
        {
            HttpRuntime.UnloadAppDomain();
            return null;
        }
        
        public ActionResult CacheKeys()
        {
            var sb = new StringBuilder();
            var enumerator = HttpContext.Cache.GetEnumerator();

            while (enumerator.MoveNext())
            {
                sb.AppendLine((string)enumerator.Key);
            }

            return Content(sb.ToString());
        }
        

        public ActionResult Developer()
        {
            var isNowDeveloper = false;

            HttpCookie myCookie = new HttpCookie(Constants.DeveloperCookieName);
            DateTime now = DateTime.Now;

            // Set the cookie value.
            myCookie.Value = Guid.NewGuid().ToString();

            if (Request.Cookies[Constants.DeveloperCookieName] == null)
            {
                myCookie.Expires = now.AddYears(1);
                isNowDeveloper = true;
            }
            else
            {
                myCookie.Expires = now.AddMinutes(-30);
                isNowDeveloper = false;
            }

            // Add the cookie.
            Response.Cookies.Add(myCookie);

            if (isNowDeveloper)
                return Content("You're a developer");

            return Content("You're not a developer");
        }
    }
}