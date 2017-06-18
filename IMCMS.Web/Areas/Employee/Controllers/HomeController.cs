using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.Areas.Employee.Helpers;
using IMCMS.Web.Areas.Employee.ViewModel;
using IMCMS.Common.Attributes;
using IMCMS.Web.Controllers;

namespace IMCMS.Web.Areas.Employee.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IPortalSettingsPageRepository _repo;
        private readonly IVersionableRepository<EmployeePortalSettings> _setRepo;

        public HomeController(IPortalSettingsPageRepository repo, IVersionableRepository<EmployeePortalSettings> setrepo, IUnitOfWork uow)
        {
            _repo = repo;
            _setRepo = setrepo;
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            ActionResult result = CheckUser();

            if (result != null)
            {
                filterContext.Result = result;
            }
        }

        [NonAction]
        private ActionResult CheckUser()
        {
            if (!this.IsUserAuthenticated())
            {
                return RedirectToAction("Login", "Account", new { ReturnUrl = Uri.EscapeDataString(Request.Url.AbsolutePath) });
            }
            return null;
        }

        public ActionResult Index(string slug)
        {

            if (!string.IsNullOrEmpty(slug))
            {
                EmployeePortalPage page = _repo.GetPageBySlug(slug);
                if (page != null)
                {
                    if (page.PageType == PortalType.Redirect) return Redirect(page.RedirectUrl);
                    else
                    {
                        
                        if (page.ParentId == null)
                        {
                            string nslug = (page.Children.Any() ? page.Children.Where(x => x.Status == VersionableItemStatus.Live && x.Visbility != VersionableVisbility.Unpublished).OrderBy(x => x.Order).FirstOrDefault().Slug : string.Empty);
                            if (string.IsNullOrEmpty(nslug)) return HttpNotFound();
                            else return RedirectToAction("Index", new { slug = nslug });
                        }
                        return View("Detail", page);
                    }
                }
            }

            EmployeePortalIndexViewModel model = new EmployeePortalIndexViewModel();
            var settings = _setRepo.GetAll().FirstOrDefault(x => x.BaseID == 1 && x.Status == VersionableItemStatus.Live);
            model.Headline = settings.Headline;
            model.Description = settings.Description;

            var parents = _repo.GetAll().Where(x => x.ParentId == null && x.Status == VersionableItemStatus.Live);
            model.Pages = parents.ToList();
            
            return View(model);
        }
    }
}