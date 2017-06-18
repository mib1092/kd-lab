using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Controllers
{
  [RoutePrefix("projects")]
  public class ProjectsController : BaseController
  {
    [Route("")]
    public ActionResult Index()
    {
      return View();
    }
  }
}