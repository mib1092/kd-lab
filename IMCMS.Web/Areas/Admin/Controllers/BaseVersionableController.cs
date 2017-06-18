using System;
using System.Web.Mvc;
using IMCMS.Common.Controllers;
using IMCMS.Models;
using System.Linq;
using System.Web;
using System.Collections.Generic;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using System.Reflection;
using IMCMS.Models.DAL;
using IMCMS.Common.Utility;
using IMCMS.Common.Extensions;
using System.Transactions;
using System.Data.Entity.Validation;
using IMCMS.Web.Areas.Admin.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Implement basic CRUD operations on a verionable object for an administrative area
    /// Depends on IUnitOfWork
    /// </summary>
    /// <typeparam name="T">IVersionable object</typeparam>
    public abstract class BaseVersionableController<T> : BaseAdminController
        where T : class, IVersionable, new()
    {
        protected IVersionableRepository<T> _repo;

        public BaseVersionableController(IUnitOfWork UOW, IVersionableRepository<T> repo) : base(UOW)
        {
            _repo = repo;
        }


        public virtual ActionResult Index()
        {
            return View(new AdminBaseViewModel<IEnumerable<T>> { Item = _repo.GetAll().ToList() });
        }

        public virtual ActionResult Create()
        {
            var ob = CreateBlankObject();
            OnEditRendering(ob);
            return View("Edit", OnCreatingViewModel(ob));
        }

        [HttpPost]
        public virtual ActionResult Create(AdminBaseViewModel<T> ob, FormCollection form)
        {
            return Save(null, ob.Item, form);
        }

        public virtual ActionResult Edit(int id)
        {
            T content;
            try
            {
                if (!String.IsNullOrEmpty(Request.QueryString["r"]))
                {

                    int requestedID = int.Parse(Request.QueryString["r"]);
                    content = _repo.FindBy(d => d.ID == requestedID).First();
                    DisplayRollbackWarning();
                    OnLoadRollback(content);

                }
                else
                {

                    content = _repo.GetLive(id);

                }
            }
            catch (InvalidOperationException)
            {
                throw new HttpException(404, "Object not found");
            }

            if (content == null)
                throw new HttpException(404, "Object not found");


            ViewBag.History = _repo.GetVersions(id).ToList();


            OnEditRendering(content);
            return View(OnCreatingViewModel(content));
        }

        [HttpPost]
        public virtual ActionResult Edit(int id, AdminBaseViewModel<T> ob, FormCollection form)
        {
            return Save(id, ob.Item, form);
        }


        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required))
            {
                var entity = _repo.GetLive(id);

                OnDeleting(entity);


                _repo.Delete(entity);
                _uow.Commit();
                scope.Complete();

            }
            return Json(new { status = 0 });
        }

        [HttpPost]
        public virtual ActionResult Undo(int id)
        {

            _repo.Undelete(id);
            _uow.Commit();

            return Json(new { status = 0 });
        }

        /// <summary>
        /// method that acutally saves from Edit and Create
        /// </summary>
        /// <param name="id">Base ID of object being saved. Null if new</param>
        /// <param name="ob">Object to be saved</param>
        /// <param name="form">Form collection</param>
        /// <returns>Action Result to be passed back to the user</returns>
        private ActionResult Save(int? id, T ob, FormCollection form)
        {
            try
            {
                using (var scope = new TransactionScope(TransactionScopeOption.Required))
                {
                    _repo.Add(ob, id);

                    OnSaving(ob, form, id.HasValue ? EditingType.Edit : EditingType.Insert);


                    if (ModelState.IsValid)
                    {
                        var slug = GenerateSlug(ob);
                        if (slug.IsNotNullOrEmpty())
                            ob.Slug = slug;


                        if (id.HasValue)
                            _repo.Edit(entity: ob, BaseID: id.Value);
                        //else
                        //_repo.Add(ob);

                        _uow.Commit();

                        OnSaved(ob, form);
                        _uow.Commit();


                        var actionResult = OnSaveFinished(SaveResult.Success, ob);

                        if (id.HasValue)
                            ModifiedItem();
                        else
                            CreatedItem();

                        scope.Complete();

                        return actionResult;
                    }
                    else
                    {
                        OnEditRendering(ob);
                        return OnSaveFinished(SaveResult.Fail, ob);
                    }
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationError in ex.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    RaiseError(String.Format("Property: {0} Error: {1}\n", validationError.PropertyName, validationError.ErrorMessage));
                }

                RaiseError(ex);

                OnEditRendering(ob);
                return OnSaveFinished(SaveResult.Fail, ob);
            }
            catch (Exception ex)
            {
                RaiseError(ex);
                OnEditRendering(ob);
                return OnSaveFinished(SaveResult.Fail, ob);
            }
        }

        /// <summary>
        /// Create a blank object for the Create method
        /// </summary>
        /// <returns>New blank object from the data context/repository</returns>
        private T CreateBlankObject()
        {
            var entity = new T();
            OnCreatingBlankObject(entity);
            return entity;
        }

        /// <summary>
        /// Called after all saving has finished and user action result is being created to be sent to the UI
        /// Used to override the default behavior when redirect after successful save, or if nonstandard view names are used
        /// </summary>
        /// <param name="result">Result of the save, success or fail</param>
        /// <param name="entity">Entity that was saved</param>
        public virtual ActionResult OnSaveFinished(SaveResult result, T entity)
        {
            SetUrls(entity);

            if (result == SaveResult.Success)
            {
                return RedirectToAction("Edit", new { id = entity.BaseID });
            }

            return View("Edit", OnCreatingViewModel(entity));
        }

        /// <summary>
        /// Gets the name of properties of the IVersionable object that will used to generate the slug
        /// Multiple properties are supported here, but they must be able to be casted to a string
        /// </summary>
        public virtual string[] SlugFields
        {
            get
            {
                return new string[] { "Title" };
            }
        }

        /// <summary>
        /// Uses the properties in SlugFields to generate a slug. 
        /// If you override this, you must return a completed slug (i.e. 'test-object')
        /// If you return null or empty, the slug will not be updated
        /// </summary>
        /// <param name="obj">object that should be searched for properties</param>
        /// <returns>Slug to be saved to database</returns>
        public virtual string GenerateSlug(T obj)
        {

            var type = obj.GetType();
            var properties = type.GetProperties().Where(d => SlugFields.Contains(d.Name)).ToArray();

            ICollection<string> slugValues = properties.Select(property => property.GetValue(obj, null).ToString()).ToList();

            return Slug.Create(true, slugValues.ToArray());

        }

        /// <summary>
        /// Called right before the ModelState is checked, but within the same SQL transaction
        /// </summary>
        /// <param name="obj">The object about to be saved</param>
        /// <param name="form">The HTTP Form collection of values</param>
        /// <param name="type">Editing type, Inserting or Editing the object</param>
        protected virtual void OnSaving(T obj, FormCollection form, EditingType type)
        {
        }

        /// <summary>
        /// Called after the commit of main versionable object before the SQL transaction is commited/completed to prevent hanging objects in the event of errors
        /// Commit to the database is called automatically
        /// Useful if you need to set related data where you need an ID of the versionable object
        /// </summary>
        /// <param name="obj">Object that was saved</param>
        /// <param name="form">Form collection allowing direct access to data</param>
        protected virtual void OnSaved(T obj, FormCollection form)
        {
        }

        /// <summary>
        /// Called before the edit page is rendered. Create and Edit specifically
        /// </summary>
        /// <param name="obj">The object to be edited</param>
        protected virtual void OnEditRendering(T obj)
        {
            SetUrls(obj);
        }

        /// <summary>
        /// Called before a delete happens.
        /// Examples of use: santiy check before delete, do some clean up, ensure its allowed to be deleted
        /// </summary>
        /// <param name="obj">Object to be deleted</param>
        protected virtual void OnDeleting(T obj)
        {
        }

        protected virtual void SetUrls(T obj)
        {
            SetViewPage(GetEndUserPage(obj));
            SetListPage(GetListPage(obj));
        }


        /// <summary>
        /// Get's the list page for the current object
        /// </summary>
        /// <returns></returns>
        protected virtual string GetListPage(T obj)
        {
            return Url.Action("Index");
        }

        /// <summary>
        /// Get's the end user page for the current object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual string GetEndUserPage(T obj)
        {
            return Url.Action("Detail", GetType().Name.Replace("Controller", ""), new { obj.Slug, area = "" });
        }

        /// <summary>
        /// Called when a previous version of an object is loaded to the edit page
        /// </summary>
        /// <param name="obj">The previous object</param>
        protected virtual void OnLoadRollback(T obj)
        {
            var live = _repo.GetLive(obj.BaseID);
        }

        /// <summary>
        /// On creation of a new blank object for the Create page.
        /// Useful if you need to set a default value, or value that is not editable by the end user on the Create page
        /// Do not call Commit/SaveChanges in this event. This will cause a blank entity to be inserted into the database
        /// </summary>
        /// <param name="obj">Blank object</param>
        protected virtual void OnCreatingBlankObject(T obj)
        {
        }

        protected virtual AdminBaseViewModel<T> OnCreatingViewModel(T obj)
        {
            return new AdminBaseViewModel<T> { Item = obj };
        }
    }

    public enum EditingType
    {
        Insert,
        Edit
    }

    public enum SaveResult
    {
        Success,
        Fail
    }
}
