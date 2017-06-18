using IMCMS.Models.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class DefaultController : BaseAdminController
    {
        public DefaultController(IUnitOfWork uow) : base(uow)
        {

        }

        public ActionResult Index()
        {
            return Redirect("/");
        }
    }
}