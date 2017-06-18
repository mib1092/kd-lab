using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Text.RegularExpressions;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.Areas.Admin.ViewModels;
using IMCMS.Common.Utility;
using StackExchange.Profiling;
using Newtonsoft.Json.Linq;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class EmployeePortalController : BaseVersionableController<EmployeePortalPage>
    {
        private readonly IVersionableRepository<EmployeePortalSettings> _settingsRepo;

        public EmployeePortalController(IUnitOfWork uow, IVersionableRepository<EmployeePortalPage> repo) : base(uow, repo)
        {
            _settingsRepo = new VersionableRepository<EmployeePortalSettings>((DbContext)uow);
        }

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Portal;
            }
        }

        public ActionResult ResetPassword()
        {
            var item = _settingsRepo.GetAll().FirstOrDefault(x => x.BaseID == 1 && x.Status == VersionableItemStatus.Live);
            string password = "W0ZhGbuv";
            item.Password = Common.Hashing.AESEncrypt.Encrypt(password);
            _uow.Commit();
            return RedirectToAction("Index");
        }

        public override ActionResult Index()
        {
            EmployeePortalSettings content;

            if (!String.IsNullOrEmpty(Request.QueryString["r"]))
            {
                int requestedID = int.Parse(Request.QueryString["r"]);
                content = _settingsRepo.FindBy(d => d.ID == requestedID).First();
                DisplayRollbackWarning();
                var live = _settingsRepo.GetLive(content.BaseID);
            }
            else
            {
                content = _settingsRepo.GetLive(1);
            }

            var viewModel = new EmployeePortalViewModel();

            viewModel.Item = content;
            viewModel.Pages = _repo.GetAll().Where(x => x.ParentId == null && x.Status == VersionableItemStatus.Live).ToList();

            ViewBag.History = _settingsRepo.GetVersions(1).ToList();

            return View(viewModel);

        }

        [HttpPost]
        public virtual ActionResult Index(EmployeePortalViewModel ob, FormCollection form)
        {
            ob.Pages = _repo.GetAll().Where(x => x.ParentId == null && x.Status == VersionableItemStatus.Live).ToList();


            try
            {
                var obj = ob.Item;

                _settingsRepo.Add(obj, 1);

                obj.Password = IMCMS.Common.Hashing.AESEncrypt.Encrypt(obj.Password);

                if (ModelState.IsValid)
                {
                    _settingsRepo.Edit(entity: obj, BaseID: 1);
                    _uow.Commit();
                    ModifiedItem();
                }
            }
            catch (Exception ex)
            {
                RaiseError(ex);
            }

            ViewBag.History = _settingsRepo.GetVersions(1).ToList();

            return View(ob);
        }

        protected override void OnCreatingBlankObject(EmployeePortalPage obj)
        {
            base.OnCreatingBlankObject(obj);
            obj.PageType = PortalType.Standard;
            if (HttpContext.Request.QueryString["t"] != null)
            {
                obj.PageType = PortalType.Redirect;
                obj.IsNewWindow = false;
            }
        }

        public override ActionResult Create()
        {
            var parents = _repo.GetAll().Where(x => x.ParentId == null && x.Status == VersionableItemStatus.Live);
            var list = new List<SelectListItem>();
            foreach (var item in parents.OrderBy(x => x.Order))
            {
                list.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Title });
            }
            ViewBag.ParentSelect = list;
            return base.Create();
        }

        public override ActionResult Edit(int id)
        {
            var parents = _repo.GetAll().Where(x => x.ParentId == null && x.Status == VersionableItemStatus.Live);
            var list = new List<SelectListItem>();
            foreach (var item in parents.OrderBy(x => x.Order))
            {
                list.Add(new SelectListItem() { Value = item.ID.ToString(), Text = item.Title });
            }
            ViewBag.ParentSelect = list;
            return base.Edit(id);
        }

        int previousID = 0;
        protected override void OnSaving(EmployeePortalPage obj, FormCollection form, EditingType type)
        {
            base.OnSaving(obj, form, type);
            if (type == EditingType.Edit)
            {
                if (obj.ParentId == null)
                {
                    var curObj = _repo.GetLive(obj.BaseID);
                    previousID = curObj.ID;
                    obj.Slug = GenerateSlug(obj);
                }
            }

            if (obj.PageType == PortalType.Redirect)
            {
                obj.RedirectUrl = CheckURL(obj.RedirectUrl);
            }

            using (MiniProfiler.Current.Step("duplicate title check"))
            {
                if (_repo.FindBy(x => x.Title == obj.Title && x.BaseID != obj.BaseID && x.ParentId == obj.ParentId && x.Status == VersionableItemStatus.Live).Count() > 0)
                {
                    ModelState.AddModelError("DuplicateTitles", "Duplicate page titles on the same level. Please rename a page");
                }
            }
        }

        private string CheckURL(string url)
        {
            string chkurl = url;
            string pattern = @"^(http|https|mailto|ftp|tel)";
            Regex reg = new Regex(pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (!string.IsNullOrEmpty(chkurl) && !reg.IsMatch(url) && chkurl[0] != '/')
            {
                chkurl = "/" + chkurl;
            }
            return chkurl;
        }

        protected override void OnSaved(EmployeePortalPage obj, FormCollection form)
        {
            base.OnSaved(obj, form);

            if (previousID > 0)
            {
                var newLiveObj = _repo.GetLive(obj.BaseID);
                var needUpdated = _repo.FindBy(x => x.ParentId == previousID).ToList();

                foreach (var item in needUpdated)
                {
                    item.ParentId = newLiveObj.ID;
                    item.Slug = GenerateSlug(item);
                }
            }
        }

        public ActionResult Order(FormCollection form)
        {
            JObject j = JObject.Parse(form["Data"]);
            var p = _repo.GetAll().ToList();
            try
            {
                GenerateOrder(p, j["data"]);
                _uow.Commit();
            }
            catch (ApplicationException ex)
            {
                return new HttpStatusCodeResult(409);
            }

            return Json(new { status = 0 });
        }

        private void GenerateOrder(IEnumerable<EmployeePortalPage> obj, JToken list)
        {
            for (int i = 0; i < list.Count(); i++)
            {
                int parentId = int.Parse(list[i]["id"].ToString());
                EmployeePortalPage parent = obj.FirstOrDefault(c => c.BaseID == parentId && c.Status == VersionableItemStatus.Live);

                if (list[i]["children"] != null && parent != null)
                {
                    var clist = list[i]["children"];
                    for (int j = 0; j < clist.Count(); j++)
                    {
                        int childId = int.Parse(clist[j]["id"].ToString());
                        var item = obj.SingleOrDefault(c => c.BaseID == childId && c.Status == VersionableItemStatus.Live);
                        
                        if (item != null)
                        {
                            item.Order = j + 1;
                            item.ParentId = parent.BaseID;
                            item.Slug = String.Format("{0}/{1}", parent.Slug, Slug.Create(true, item.Title));
                        }
                    }
                }
            }
        }

        public override string GenerateSlug(EmployeePortalPage obj)
        {
            if (obj.ParentId != null)
            {
                var parent = _repo.FindBy(x => x.Status == VersionableItemStatus.Live && x.ID == obj.ParentId).FirstOrDefault();
                return string.Format("{0}/{1}", parent.Slug, Slug.Create(true, obj.Title));
            }

            return Slug.Create(true, obj.Title);
        }
    }
}