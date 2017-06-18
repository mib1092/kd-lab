using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Controllers
{
    [RoutePrefix("error")]
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        
            [Route("Index"), Route("")]
        public ActionResult Index()
        {
            Response.StatusCode = 404;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

        [Route("ServerError")]
        public ActionResult ServerError()
        {
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
            return View();
        }

    }
}
