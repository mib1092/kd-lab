using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Xml;
using System.Globalization;
using System.Configuration;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Common.Extensions;
using IMCMS.Web.ViewModels;
using Newtonsoft.Json;
using System.Net.Mail;
using Postal;

namespace IMCMS.Web.Controllers
{
    [RoutePrefix("careers")]
    public class CareersController : BaseController
    {
        private JobRepository _jobRepo;
        private Repository<JobApplication> _appRepo;

        public CareersController(IUnitOfWork uow)
        {
            _jobRepo = new JobRepository((DbContext)uow);
            _appRepo = new Repository<JobApplication>((DbContext)uow);
        }

        private List<SelectListItem> JobList()
        {
            List<SelectListItem> output = new List<SelectListItem>();

            foreach (var item in _jobRepo.GetAll().Where(x => x.IsDefault).OrderBy(x => x.Order))
            {
                output.Add(new SelectListItem() { Text = item.Title, Value = item.ID.ToString() });
            }
            
            var list = _jobRepo.GetAll().Where(x => !x.IsDefault && x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public).OrderBy(x => x.Order);
            foreach (var item in list)
            {
                output.Add(new SelectListItem() { Text = item.Title, Value = item.ID.ToString() });
            }
            return output;
        }

        [Route("{slug?}")]
        public ActionResult Index(string slug)
        {
            CareersViewModel viewModel = new CareersViewModel();
            if (string.IsNullOrEmpty(slug))
            {
                viewModel.Jobs = _jobRepo.GetAllPublic();
                viewModel.JobList = JobList();
                viewModel.JobApp = new JobApplication() { State = "" };
                viewModel.IsApplyView = false;
                return View(viewModel);
            }

            return View("Detail", new BaseViewModel<Job>() { Item = _jobRepo.GetBySlug(slug) });

        }

        [Route("apply/{slug}")]
        public ActionResult Apply(string slug)
        {
            CareersViewModel viewModel = new CareersViewModel();
            viewModel.Jobs = _jobRepo.GetAllPublic();
            viewModel.JobList = JobList();
            int jobID = 0;
            var item = _jobRepo.GetBySlug(slug);
            if (item != null) jobID = item.ID;
            viewModel.IsApplyView = true;
            viewModel.JobApp = new JobApplication() { JobID = jobID, State = "" };
            return View("Apply", viewModel);
        }

        [Route("indeed.xml")]
        public ActionResult Indeed()
        {
            var jobs = _jobRepo.GetIndeedJobs();

            XmlDocument doc = new XmlDocument();
            XmlElement el = (XmlElement)doc.AppendChild(doc.CreateElement("Source"));
            el.AppendChild(doc.CreateElement("publisher")).InnerText = "Kolde Construction, Inc.";
            el.AppendChild(doc.CreateElement("publisherurl")).InnerText = "http://koldeconcrete.com/";

            foreach (Job j in jobs)
            {
                XmlElement xe = doc.CreateElement("job");

                xe.AppendChild(doc.CreateElement("title")).InnerXml = "<![CDATA[" + j.Title + "]]>";

                xe.AppendChild(doc.CreateElement("date")).InnerXml = "<![CDATA[" + j.IndeedDate.Value.AddHours(6).ToString(CultureInfo.InvariantCulture) + "]]>";

                xe.AppendChild(doc.CreateElement("referencenumber")).InnerXml = "<![CDATA[" + j.IndeedRef + "]]>";

                if (Request.Url != null)
                    xe.AppendChild(doc.CreateElement("url")).InnerXml = "<![CDATA[http://" + Request.Url.Host + Url.Action("Detail", "Careers",
                        new { id = j.BaseID, slug = j.Slug }) + "]]>";

                if (!String.IsNullOrEmpty(j.City))
                    xe.AppendChild(doc.CreateElement("city")).InnerXml = "<![CDATA[" + j.City + "]]>";

                xe.AppendChild(doc.CreateElement("company")).InnerXml = "<![CDATA[Wolf Construction]]>";

                if (!String.IsNullOrEmpty(j.PostalCode))
                    xe.AppendChild(doc.CreateElement("postalcode")).InnerXml = "<![CDATA[ " + j.PostalCode + "]]>";

                if (!String.IsNullOrEmpty(j.State))
                    xe.AppendChild(doc.CreateElement("state")).InnerXml = "<![CDATA[" + j.State + "]]>";

                xe.AppendChild(doc.CreateElement("country")).InnerXml = "<![CDATA[US]]>";

                xe.AppendChild(doc.CreateElement("description")).InnerXml = "<![CDATA[ " + j.Description.StripHtml() + "]]>";
                if (!String.IsNullOrEmpty(j.Wage))
                    xe.AppendChild(doc.CreateElement("salary")).InnerXml = "<![CDATA[ " + j.Wage + "]]>";
                if (!String.IsNullOrEmpty(j.Education))
                    xe.AppendChild(doc.CreateElement("education")).InnerXml = "<![CDATA[ " + j.Education + "]]>";
                if (!String.IsNullOrEmpty(j.Hours))
                    xe.AppendChild(doc.CreateElement("jobtype")).InnerXml = "<![CDATA[ " + j.Hours + "]]>";
                if (!String.IsNullOrEmpty(j.Category))
                    xe.AppendChild(doc.CreateElement("category")).InnerXml = "<![CDATA[ " + j.Category + "]]>";
                if (!String.IsNullOrEmpty(j.Experience))
                    xe.AppendChild(doc.CreateElement("experience")).InnerXml = "<![CDATA[ " + j.Experience + "]]>";

                el.AppendChild(xe);
            }
            return Content(doc.OuterXml, "text/xml");

        }

        [HttpPost]
        [Route("careers/GetJobType")]
        public ActionResult GetJobType(FormCollection form)
        {
            string type = "unknown";
            int id = 0;

            if (form["id"] != null) int.TryParse(form["id"].ToString(), out id);
            if (id > 0)
            {
                var item = _jobRepo.FindBy(x => x.ID == id).FirstOrDefault();
                if (item != null)
                {
                    type = item.JobType.Value.ToString();
                }
            }
            else
            {
                if (form["text"] != null) type = form["text"].ToString().Replace("Other - ", "").Trim();
            }

            return Json(new { type = type });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Route("careers/application")]
        public ActionResult Application(CareersViewModel obj, FormCollection form)
        {
            var item = obj.JobApp;
            item.Submitted = DateTime.Now;

            int month = 0,
                day = 0,
                year = 0;
            if (form["Month"] != null) int.TryParse(form["Month"].ToString(), out month);
            if (form["Day"] != null) int.TryParse(form["Day"].ToString(), out day);
            if (form["Year"] != null) int.TryParse(form["Year"].ToString(), out year);

            var job = _jobRepo.FindBy(x => x.ID == item.JobID).FirstOrDefault();
            if (job != null) item.AppJobID = job.JobID;

            _uow.JobApplications.Add(item);
            _uow.Commit();

            dynamic email = new Email("JobApplication");
            email.To = ConfigurationManager.AppSettings["JobApplicationEmail"];
            email.Bcc = ConfigurationManager.AppSettings["DebugEmail"];
            email.Model = item;
            email.Send();

            //return Newtonsoft.Json.JsonConvert.SerializeObject(item);
            return View("ApplicationSubmitted");
        }

        
    }
}