using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using StackExchange.Profiling;
using IMCMS.Web.Areas.Admin.ViewModels;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class CareersController : BaseVersionableController<Job>
    {

        private readonly Repository<JobApplication> _appRepo;

        public CareersController(IUnitOfWork uow, IVersionableRepository<Job> repo) : base (uow, repo)
        {
            _appRepo = new Repository<JobApplication>((DbContext)uow);
        }

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Careers;
            }
        }

        protected override void OnCreatingBlankObject(Job obj)
        {
            base.OnCreatingBlankObject(obj);
            obj.State = "KS";
            obj.ListOnWebsite = true;
            obj.IndeedDate = DateTime.Today;
            obj.IndeedRef = DateTime.Now.ToString("yyyyMMddHHmmss");
            obj.Order = int.MaxValue;
        }

        protected override AdminBaseViewModel<Job> OnCreatingViewModel(Job obj)
        {
            var ob = base.OnCreatingViewModel(obj);
            if (!ob.Item.ListOnIndeed)
            {
                ob.Item.IndeedDate = DateTime.Today;
                ob.Item.IndeedRef = DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            return ob;
        }

        int previousID = 0;
        string jobID = string.Empty;
        protected override void OnSaving(Job obj, FormCollection form, EditingType type)
        {
            base.OnSaving(obj, form, type);

            if (type == EditingType.Edit)
            {
                var curObj = _repo.GetLive(obj.BaseID);
                previousID = curObj.ID;
                jobID = curObj.JobID;
            }

            if (obj.ListOnIndeed && string.IsNullOrEmpty(obj.IndeedRef))
            {
                if (obj.IndeedDate == null) obj.IndeedDate = DateTime.Now;
                obj.IndeedRef = String.Format("{0}{1}", obj.IndeedDate.Value.ToString("yyyymmdd"), obj.BaseID);
            }

            using (MiniProfiler.Current.Step("duplicate title check"))
            {
                if (_repo.FindBy(x => x.Title == obj.Title && x.BaseID != obj.BaseID && x.Status == VersionableItemStatus.Live).Count() > 0)
                {
                    ModelState.AddModelError("DuplicateTitles", "Duplicate job titles. Please rename a job.");
                }
            }
        }

        protected override void OnSaved(Job obj, FormCollection form)
        {
            base.OnSaved(obj, form);

            if (previousID > 0)
            {
                var newLiveObj = _repo.GetLive(obj.BaseID);
                var needUpdated = _appRepo.FindBy(x => x.JobID == previousID && x.AppJobID == jobID).ToList();

                foreach (var item in needUpdated)
                {
                    item.JobID = newLiveObj.ID;
                }
            }
        }

        [HttpPost]
        public ActionResult Order(FormCollection form)
        {
            int i = 0;
            var all = _repo.GetAll();
            var ids = form["row"].Split(',');

            foreach (var item in ids)
            {
                int id = 0;
                int.TryParse(item, out id);
                all.First(x => x.ID == id).Order = 1;
                i++;
            }
            _uow.Commit();

            return Json(new { status = 0 });
        }
    }
}