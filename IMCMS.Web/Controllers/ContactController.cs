using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMCMS.Web.ViewModels;
using IMCMS.Models.DAL;
using IMCMS.Models.Repository;
using IMCMS.Models.Entities;
using System.Net.Mail;
using Postal;

namespace IMCMS.Web.Controllers
{
    [RoutePrefix("contact")]
    public class ContactController : BaseController
    {
        [Route("")]
        public ActionResult Index()
        {
            return View(new BaseViewModel<Contact>());
        }

        [HttpPost, Route("")]
        public ActionResult Index(BaseViewModel<Contact> obj, FormCollection form, string[] Contact_Types)
        {
            var repo = new Repository<Contact>((System.Data.Entity.DbContext)_uow);
            obj.Item.Submitted = DateTime.Now;
            obj.Item.Options = (Contact_Types == null ? string.Empty : String.Join(", ", Contact_Types));
            repo.Add(obj.Item);
            _uow.Commit();

            dynamic email = new Email("Contact");
            email.Model = obj.Item;
            try
            {
                email.Send();
            }
            catch (SmtpException ex)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return View("Thanks");
        }
    }
}