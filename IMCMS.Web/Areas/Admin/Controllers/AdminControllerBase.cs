using IMCMS.Common.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using IMCMS.Web.ViewModels;
using IMCMS.Models.DAL;
using IMCMS.Models.Repository;
using System.Data.Entity;
using IMCMS.Web.Areas.Admin.ViewModels;
using IMCMS.Models.Services;
using StackExchange.Profiling;
using IMCMS.Models.Entities;
//using IMCMS.Common.Attributes;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    [ValidateInput(false)]
    [GlobalValidateAntiForgeryToken(HttpVerbs.Post)]
    [Authorize]
    public class BaseAdminController : Controller
    {
        protected readonly IUnitOfWork _uow;
        private readonly Repository<JobApplication> _jobRepo;

        public BaseAdminController(IUnitOfWork uow)
        {
            _uow = uow;
            _jobRepo = new Repository<JobApplication>((DbContext)uow);
        }

        public virtual ActiveSection? AdminBarActiveSection
        {
            get
            {
                return null;
            }
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);


            var viewResult = filterContext.Result as ViewResult;
            if (viewResult != null)
            {
                var model = (viewResult.ViewData.Model as AdminBaseViewModel) ?? new AdminBaseViewModel();

                model.Error = TempData["ErrorMessage"] as string;
                model.ListPage = TempData["ListPage"] as string;
                model.ViewPage = TempData["ViewPage"] as string;
                model.Success = (TempData["Created"] != null || TempData["Edited"] != null);
                model.IsRollback = _rollback;

                model.AdminBar.ActiveSection = AdminBarActiveSection;
                model.AdminBar.NewAppCount = _jobRepo.GetAll().Where(x => x.Status == ApplicationStatus.New).Count();
                viewResult.ViewData.Model = model;
            }
        }

        bool _rollback = false;

        /// <summary>
        /// Notify user that an item has been created
        /// </summary>
        protected void CreatedItem()
        {
            TempData["Created"] = true;
        }

        protected void ModifiedItem()
        {
            TempData["Edited"] = true;
        }

        protected void RaiseError(string Error)
        {
            TempData["ErrorMessage"] = Error;
        }

        protected void RaiseError(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            RaiseError(ex.ToString());
        }

        protected void DisplayRollbackWarning()
        {
            _rollback = true;
        }

        protected void SetViewPage(string Url)
        {
            TempData["ViewPage"] = Url;
        }

        protected void SetListPage(string Url)
        {
            TempData["ListPage"] = Url;
        }
    }
}
