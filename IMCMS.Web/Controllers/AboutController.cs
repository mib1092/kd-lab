using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Controllers
{
    [RoutePrefix("about")]
    public class AboutController : BaseController
    {
        private VersionableRepository<TeamMember> _teamRepo;

        public AboutController(IUnitOfWork uow)
        {
            _teamRepo = new VersionableRepository<TeamMember>((DbContext)uow);
        }

        [Route("")]
        public ActionResult Index()
        {
            var model = new BaseViewModel<IEnumerable<TeamMember>>();
            model.Item = _teamRepo.GetAll().Where(x => x.Status == VersionableItemStatus.Live);
            return View(model);
        }

        
    }
}