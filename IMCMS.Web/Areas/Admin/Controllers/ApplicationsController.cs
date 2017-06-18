using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data.Entity;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Models.Repository;
using IMCMS.Web.Areas.Admin.ViewModels;
using IMCMS.Common;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using Postal;
using OfficeOpenXml;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Admin.Controllers
{
    public class ApplicationsController : BaseAdminController
    {
        private readonly Repository<JobApplication> _repo;

        public ApplicationsController(IUnitOfWork uow) : base(uow)
        {
            _repo = new Repository<JobApplication>((DbContext)uow);
        }

        public override ActiveSection? AdminBarActiveSection
        {
            get
            {
                return ActiveSection.Applications;
            }
        }

        public ActionResult Index()
        {
            var appStatus = ApplicationStatus.New;
            var jobCat = "nondriving".Equals(Request.QueryString["t"], StringComparison.OrdinalIgnoreCase) ? JobType.Non_Driving : JobType.Driving;

            if (Request.QueryString["s"] != null) appStatus = GetStatus(Request.QueryString["s"].ToString());

            ViewBag.JobCategory = jobCat;
            ViewBag.ApplicationStatus = appStatus;
            ViewBag.NewCount = _repo.GetAll().Where(x => x.Job.JobType == jobCat && x.Status == ApplicationStatus.New).Count();
            var model = new AdminBaseViewModel<List<JobApplication>> { Item = _repo.GetAll().Where(x => x.Status == appStatus && x.Job.JobType == jobCat).OrderByDescending(x => x.Submitted).ToList() };

            return View(model);
        }

        public ActionResult Details(int id)
        {
            var model = new AdminBaseViewModel<JobApplication>();
            var ob = _repo.FindBy(x => x.ID == id).FirstOrDefault();
            model.Item = ob;
            SetListPage(Url.Action("Index", new { t = ob.Job.JobType.ToString().Replace("_", "").ToLowerInvariant(), s = ob.Status.ToString().ToLowerInvariant() }));
            return View(model);
        }

        [HttpPost]
        public ActionResult Details(int id, AdminBaseViewModel<JobApplication> obj, FormCollection form)
        {
            var ob = _repo.FindBy(x => x.ID == id).FirstOrDefault();
            ob.Notes = obj.Item.Notes;
            ob.Status = obj.Item.Status;
            _uow.Commit();

            ModifiedItem();
            
            SetListPage(Url.Action("Index", new { t = ob.Job.JobType.ToString().Replace("_","").ToLowerInvariant(), s = ob.Status.ToString().ToLowerInvariant() }));

            return RedirectToAction("Details", new AdminBaseViewModel<JobApplication> { Item = ob });
        }

        
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(FormCollection form)
        {
            JObject j = JObject.Parse(form["Data"]);
            string action = j["action"].ToString();
            var obj = j["data"];
            for (int i = 0; i < obj.Count(); i++)
            {
                int itemid = 0;
                int.TryParse(obj[i].ToString(), out itemid);
                if (itemid > 0)
                {
                    var app = _repo.FindBy(x => x.ID == itemid).FirstOrDefault();
                    if (app != null) app.Status = GetStatus(action);
                }
            }
            _uow.Commit();

            return Json(new { status = 0 });
        }

        [HttpPost]
        public JsonResult SendEmail(FormCollection form)
        {
            List<JobApplication> apps = new List<JobApplication>();
            string emailList = (form["hdnEmailList"] != null ? form["hdnEmailList"].ToString() : string.Empty);
            if (!string.IsNullOrEmpty(emailList))
            {
                foreach (var appID in emailList.Split(','))
                {
                    int id = 0;
                    int.TryParse(appID, out id);
                    if (id > 0)
                    {
                        var app = _repo.FindBy(x => x.ID == id).FirstOrDefault();
                        if (app != null) apps.Add(app);
                    }
                }
            }

            string baseMessage = (form["messageText"] != null ? form["messageText"].ToString() : string.Empty);
            bool sendAttachment = true;
            if (form["sendAttachment"] != null) bool.TryParse(form["sendAttachment"].ToString(), out sendAttachment);

            List<int> emailsSent = new List<int>();
            if (apps.Any() && !string.IsNullOrEmpty(baseMessage))
            {
                foreach (var app in apps)
                {
                    string message = "<p>Dear " + app.FirstName + " " + app.LastName + ",</p>";
                    message += Server.HtmlDecode(baseMessage);

                    dynamic email = new Email("Followup");
                    email.To = app.Email;
                    email.Bcc = ConfigurationManager.AppSettings["DebugEmail"];
                    email.Subject = "Thank You for Online Application with Kolde Construction";
                    email.Content = message;
                    if (sendAttachment)
                    {
                        string docPath = System.Web.HttpContext.Current.Server.MapPath("~/includes/documents");
                        string docFileName = "Application_for_Employment.pdf";
                        var path = System.IO.Path.Combine(docPath, docFileName);
                        email.Attach(new Attachment(path));
                    }
                    email.Send();
                }
            }

            return Json(new { Status = 0 });
        }

        public ActionResult Export()
        {
            string fileName = String.Format("Report_{0}.xlsx", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string filePath = Server.MapPath(Constants.EXCEL_DIR);
            Directory.CreateDirectory(filePath);
            filePath = Server.MapPath(Constants.EXCEL_DIR + fileName);
            FileInfo file = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage(file))
            {
                foreach (JobType type in Enum.GetValues(typeof(JobType)))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(type.ToString().Replace("_","-") + " Job Applications");

                    worksheet.Cells[1, 1].Value = "Date Submitted";
                    worksheet.Cells[1, 2].Value = "Job ID #";
                    worksheet.Cells[1, 3].Value = "Applying For";
                    worksheet.Cells[1, 4].Value = "First Name";
                    worksheet.Cells[1, 5].Value = "Last Name";
                    worksheet.Cells[1, 6].Value = "Address";
                    worksheet.Cells[1, 7].Value = "City";
                    worksheet.Cells[1, 8].Value = "State";
                    worksheet.Cells[1, 9].Value = "Zip";
                    worksheet.Cells[1, 10].Value = "Phone";
                    worksheet.Cells[1, 11].Value = "Email";
                    worksheet.Cells[1, 12].Value = "CDL Drivers License";
                    worksheet.Cells[1, 13].Value = "Felony Conviction?";
                    worksheet.Cells[1, 14].Value = "Authorized in US?";
                    worksheet.Cells[1, 15].Value = "Over 18?";
                    worksheet.Cells[1, 16].Value = "Job History";
                    worksheet.Cells[1, 17].Value = "Status";
                    worksheet.Cells[1, 18].Value = "Last Contacted";
                    worksheet.Cells[1, 19].Value = "Notes";

                    using (var range = worksheet.Cells[1, 1, 1, 20])
                    {
                        range.Style.Font.Bold = true;
                    }

                    var apps = _repo.GetAll().Where(x => x.Job.JobType == type).OrderByDescending(x => x.Submitted).ToList();

                    worksheet.InsertRow(2, apps.Count);
                    for (int i = 0; i < apps.Count; i++)
                    {
                        int rowStart = 2;
                        worksheet.Cells[i + rowStart, 1].Value = apps[i].Submitted.ToString("MM/dd/yyyy HH:mm tt");
                        worksheet.Cells[i + rowStart, 2].Value = apps[i].JobID;
                        worksheet.Cells[i + rowStart, 3].Value = apps[i].Job.Title;
                        worksheet.Cells[i + rowStart, 4].Value = apps[i].FirstName;
                        worksheet.Cells[i + rowStart, 5].Value = apps[i].LastName;
                        worksheet.Cells[i + rowStart, 6].Value = apps[i].Address;
                        worksheet.Cells[i + rowStart, 7].Value = apps[i].City;
                        worksheet.Cells[i + rowStart, 8].Value = apps[i].State;
                        worksheet.Cells[i + rowStart, 9].Value = apps[i].ZipCode;
                        worksheet.Cells[i + rowStart, 10].Value = apps[i].Phone;
                        worksheet.Cells[i + rowStart, 11].Value = apps[i].Email;
                        if (apps[i].Job.JobType == JobType.Driving)
                        {
                            worksheet.Cells[i + rowStart, 12].Value = apps[i].DriversLicense + " / " + apps[i].DriversLicenseState;
                        }
                        worksheet.Cells[i + rowStart, 13].Value = (apps[i].Felony == null ? "N/A" : apps[i].Felony.Value ? "Yes" : "No");
                        worksheet.Cells[i + rowStart, 14].Value = (apps[i].USAuthorized == null ? "N/A" : apps[i].USAuthorized.Value ? "Yes" : "No");
                        worksheet.Cells[i + rowStart, 15].Value = (apps[i].IsEighteen == null ? "N/A" : apps[i].IsEighteen.Value ? "Yes" : "No");
                        string output = string.Empty;
                        foreach (var item in apps[i].History)
                        {
                            output += item.StartDate + " - " + item.EndDate + "\r\n";
                            output += item.Company + ", " + item.Position + "\r\n";
                            output += "Description: " + item.Description + "\r\n";
                        }
                        worksheet.Cells[i + rowStart, 16].Value = output;
                        worksheet.Cells[i + rowStart, 17].Value = apps[i].Status.ToString();
                        worksheet.Cells[i + rowStart, 18].Value = (apps[i].EmailSent == null ? "" : apps[i].EmailSent.Value.ToString("MM/dd/yyyy HH:mm tt"));
                        worksheet.Cells[i + rowStart, 19].Value = apps[i].Notes;

                    }
                    worksheet.Cells.AutoFitColumns();
                }

                package.Save();
            }

            return File(filePath, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [NonAction]
        private static ApplicationStatus GetStatus(string value)
        {
            string status = (value ?? "new").ToLowerInvariant();
            ApplicationStatus appStatus = ApplicationStatus.New;
            switch (status)
            {
                case "new":
                    appStatus = ApplicationStatus.New;
                    break;
                case "approved":
                    appStatus = ApplicationStatus.Approved;
                    break;
                case "archived":
                    appStatus = ApplicationStatus.Archived;
                    break;
                case "inconsideration":
                    appStatus = ApplicationStatus.InConsideration;
                    break;
                case "prescreened":
                    appStatus = ApplicationStatus.Prescreened;
                    break;
                case "rejected":
                    appStatus = ApplicationStatus.Rejected;
                    break;
            }

            return appStatus;
        }
    }
}