using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Services;
using IMCMS.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace IMCMS.Web.Controllers
{
    public abstract class BaseVersionableController<T> : BaseController
        where T : class, IVersionable
    {
        DataContext _context;
        public BaseVersionableController() : this(new DataContext()) { }

        public BaseVersionableController(IUnitOfWork uow) : base(uow)
        {
            _context = (DataContext)uow;
        }

        /// <summary>
        /// Gets or sets an option if the index/list page a paging list
        /// </summary>
        public virtual bool IsListPaging
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the page size if the list page is pagable
        /// </summary>
        public virtual int ListPageSize
        {
            get
            {
                return 5;
            }
        }

        [Route("")]
        public virtual ActionResult Index()
        {
            int p;
            int.TryParse(Request.QueryString["p"] ?? "1", out p);

            var result = IsListPaging
                ? InternalService.GetAll(ListPageSize, p)
                : InternalService.GetAll();

            if (IsListPaging && Request.IsAjaxRequest())
            {
                return PartialView("IndexPartial", GenerateListViewModel(result, p));
            }

            return View(GenerateListViewModel(result, p));
        }

        [Route("{slug*}")]
        public virtual ActionResult Detail(string slug)
        {
            var result = InternalService.GetObject(slug, CanSeeUnpublished);

            if (result.Status == ResultStatus.NotFound)
                return HttpNotFound();

            if (result.Status == ResultStatus.MustRedirect)
                return RedirectToActionPermanent("Detail", new { result.Object.Slug });

            return View(GenerateDetailViewModel(result));
        }

        public virtual BaseViewModel<IEnumerable<T>> GenerateListViewModel(IEnumerable<T> obj, int? currentPage)
        {
            var viewModel = IsListPaging
                ? new BasePagingViewModel<IEnumerable<T>> { Item = obj.Take(ListPageSize), CurrentPage = currentPage.Value, HasMoreItems = obj.Count() > ListPageSize }
                : new BaseViewModel<IEnumerable<T>> { Item = obj };

            viewModel.AdminBar.EditURL = DetermineAdminListUrl();
            return viewModel;
        }

        public virtual VersionableObjectViewModel<T> GenerateDetailViewModel(VersionableResult<T> result)
        {
            var viewModel = new VersionableObjectViewModel<T> { Item = result.Object, Status = result.Status };
            viewModel.AdminBar.EditURL = DetermineAdminEditUrl(result.Object);
            return viewModel;
        }

        public virtual string DetermineAdminListUrl()
        {
            return Url.Action("Index", AdminControllerName, new { area = "Admin" });
        }

        public virtual string DetermineAdminEditUrl(T obj)
        {
            return Url.Action("Edit", AdminControllerName, new { area = "Admin", id = obj.BaseID });
        }

        public virtual string AdminControllerName
        {
            get
            {
                return typeof(T).Name;
            }
        }

        VersionableObjectService<T> _service;
        internal VersionableObjectService<T> InternalService { get
            {
                if (_service == null)
                    _service = GetService(_context);

                return _service;
            }
        }

        public virtual VersionableObjectService<T> GetService(DataContext db)
        {
            return new VersionableObjectService<T>(_context);
        }
    }
}