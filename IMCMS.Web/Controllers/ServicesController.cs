using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Controllers
{
  [RoutePrefix("services")]
  public class ServicesController : BaseController
  {
    [Route("")]
    public ActionResult Index()
    {
      return View();
    }
    [Route("detail")]
    public ActionResult Detail()
    {
      return View();
    }
    [Route("structural-foundations")]
    public ActionResult StructuralFoundations()
    {
      return View();
    }
  }
}