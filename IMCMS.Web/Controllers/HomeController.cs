using IMCMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        [Route("")]
        public ActionResult Index()
        {
            return View();
        }


        // mobile section
        public ActionResult FullSite()
        {
            Response.Cookies.Add(
                new HttpCookie(Constants.EscapeNativeCookieName, Constants.FullSiteValue)
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            return GenerateVersionRedirect();
        }

        public ActionResult MobileSite()
        {
            Response.Cookies.Add(
                new HttpCookie(Constants.EscapeNativeCookieName, Constants.MobileSiteValue)
                {
                    Expires = DateTime.Now.AddYears(10)
                });
            return GenerateVersionRedirect();
        }

        ActionResult GenerateVersionRedirect()
        {
            if (Request.UrlReferrer == null || !Url.IsLocalUrl(Request.UrlReferrer.AbsolutePath))
            {
                return RedirectToAction("Index", new { c = DateTime.Now.Ticks });
            }

            var url = Request.UrlReferrer.ToString();
            var builder = new UriBuilder(url);
            string queryToAppend = "c=" + DateTime.Now.Ticks;

            if (builder.Query.Length > 1) builder.Query = builder.Query.Substring(1) + "&" + queryToAppend;
            else builder.Query = queryToAppend;

            return Redirect(url);
        }

    }
}
