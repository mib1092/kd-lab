using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.Areas.Admin.ViewModels;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class FormsController : BaseAdminController
    {
        private readonly Repository<Contact> _repo;

        public FormsController(IUnitOfWork uow) : base(uow)
        {
            _repo = new Repository<Contact>((DbContext)uow);
        }

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Forms;
            }
        }

        // GET: Admin/Forms
        public ActionResult Index()
        {
            var model = new AdminBaseViewModel<List<Contact>>();
            model.Item = _repo.GetAll().OrderByDescending(x => x.Submitted).ToList();

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = new AdminBaseViewModel<Contact>();
            model.Item = _repo.FindBy(x => x.ID == id).FirstOrDefault();

            return View(model);
        }
    }
}