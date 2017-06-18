using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;
using System.Security.Cryptography;
using System.Text;
using IMCMS.Web.Areas.Employee.Helpers;
using IMCMS.Web.Areas.Employee.ViewModel;
using IMCMS.Models.Repository;
using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using IMCMS.Web.Controllers;

namespace IMCMS.Web.Areas.Employee.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IVersionableRepository<EmployeePortalSettings> _repo;

        public AccountController(IVersionableRepository<EmployeePortalSettings> repo, IUnitOfWork uow)
        {
            _repo = repo;
        }

        public ActionResult Login()
        {
            GenerateRsaInformation();
            return View(new LoginModel());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel user)
        {
            if (ModelState.IsValid)
            {
                string password = _repo.GetAll().FirstOrDefault(x => x.BaseID == 1 && x.Status == VersionableItemStatus.Live).Password;
                
                if (string.IsNullOrEmpty(password) || (IMCMS.Common.Hashing.AESEncrypt.DeCrypt(password) != user.Password))
                {
                    ModelState.AddModelError("Invalid", "Invalid password please try again.");
                    GenerateRsaInformation();
                    return View(new LoginModel());
                }

                this.LoginUser();

                string returnUrl = string.Empty;
                if (!string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                {
                    returnUrl = Server.UrlDecode(Request.QueryString["ReturnUrl"]);
                    if (returnUrl.ToLower().Replace("/employeeportal", "") == string.Empty) returnUrl = string.Empty;
                }

                if (!String.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError("Invalid", "Invalid password please try again.");
            GenerateRsaInformation();
            return View(new LoginModel());
        }

        public ActionResult LogOut()
        {
            this.LogoutUser();
            return Redirect("/");
        }

        [NonAction]
        private void GenerateRsaInformation()
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(1024);
            Session["Encryption"] = rsaProvider.ToXmlString(true);

            byte[] exponentByte = rsaProvider.ExportParameters(false).Exponent;
            byte[] modulusByte = rsaProvider.ExportParameters(false).Modulus;

            ViewData["Exponent"] = BitConverter.ToString(exponentByte).Replace("-", string.Empty);
            ViewData["Modulus"] = BitConverter.ToString(modulusByte).Replace("-", string.Empty);
        }
    }
}