using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Security.Cryptography;
using IMCMS.Common.Authentication;
using AppHarbor.Web.Security;

namespace IMCMS.Common.Controllers
{
    using System.Net.Mail;
    using System.Web.Helpers;

    using Hashing;

    using Models.DAL;
    using Models.Repository;

    using Web.Areas.Admin.Views.Account;
    using Models.Entities;
    using System.Data.Entity;
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
	public class AccountController : Controller
	{
		private IUnitOfWork _uow { get; set; }
		private IAdminUserRepository _repo { get; set; }
		private IAdminUserAttemptRepository _attemptRepo { get; set; }
		private IAdminUserSessionRepository _sessionRepo { get; set; }

		public AccountController(IAdminUserRepository repo, IAdminUserAttemptRepository attemptRepo, IUnitOfWork uow, IAdminUserSessionRepository sessionRepo)
		{
			_uow = uow;
			_repo = repo;
			_attemptRepo = attemptRepo;
			_sessionRepo = sessionRepo;
		}

		public ActionResult Login()
		{
			GenerateRsaInformation();

			Remote.PingAuthentication();

			return View();
		}

		[HttpPost]
		public ActionResult Login(LoginModel user)
		{
			if (ModelState.IsValid)
			{
				string password;

				try
				{
					var rsaProvider = new RSACryptoServiceProvider();
					rsaProvider.FromXmlString(Session["Encryption"].ToString());


					password = Encoding.ASCII.GetString(
						rsaProvider.Decrypt(Convert.FromBase64String(user.Password), false)
						);
				}
				catch (Exception ex)
				{
					ModelState.AddModelError("Invalid", "Invalid email address and/or password");
					GenerateRsaInformation();
					return View();
				}

				int? adminUserID;
				var response = AttemptLogin(user.Username, password, out adminUserID);

				if (response == AuthenticationResponse.ImagemakersSuccess
					|| response == AuthenticationResponse.LocalSuccess)
				{
					if (response == AuthenticationResponse.ImagemakersSuccess)
					{
						IAuthenticator authenticator = new CookieAuthenticator();
						var guid = authenticator.SetCookie(user.Username, false, new string[] { "IM", "Admin" });
						_sessionRepo.Add(user.Username, UserIP, guid);
					}

					if (response == AuthenticationResponse.LocalSuccess)
					{
						var dbUser = _repo.FindByEmailAddress(user.Username);

						IAuthenticator authenticator = new CookieAuthenticator();
						var guid = authenticator.SetCookie(user.Username, false, dbUser.Roles.Select(x => x.Name).ToArray());
						_sessionRepo.Add(user.Username, UserIP, guid, adminUserID);

                    }

					_uow.Commit();

                    if (!String.IsNullOrEmpty(Request.QueryString["ReturnUrl"])
							&& Url.IsLocalUrl(Request.QueryString["ReturnUrl"]))
					{
						return Redirect(Request.QueryString["ReturnUrl"]);
					}
					else
					{
						return Redirect("/");
					}
				}

				ModelState.AddModelError(
					"Invalid",
					response == AuthenticationResponse.LockedOut
						? "Looks like you've tried logging in too many times. Try again in a few minutes."
						: "Invalid username and/or password");
			}

			GenerateRsaInformation();
			return View();
		}

		public ActionResult Logout()
		{
			IAuthenticator authenticator = new CookieAuthenticator();
			authenticator.SignOut();
			return Redirect("/");
		}

		public ActionResult ForgotPassword()
		{
			return View();
		}


		[HttpPost]
		public ActionResult ForgotPassword(ForgotPasswordModel form)
		{
			if (!ModelState.IsValid)
			{
				return View();
			}

			var hash = _repo.GenerateForgotPassword(form.EmailAddress);
			_uow.Commit();

			MailMessage msg = new MailMessage();
			msg.To.Add(form.EmailAddress);
			msg.Subject = "Password Reset";
			msg.IsBodyHtml = true;
			msg.Body = String.Format(@"<p>Someone has requested to have the password reset at http://{0}.</p>

<p>If you did not request a password reset you do not need to take any action.</p>

<p>If you did, please click the link below to reset the password:<br />
<a href=""{1}"">{1}</a></p>",
Request.Url.Host,
"http://" + Request.Url.Host + "/SiteAdmin/Account/ResetPassword/" + Server.UrlEncode(hash));

			var smtp = new SmtpClient();
			try
			{
				smtp.Send(msg);
			}
			catch
			{
			}

			return View("ForgotPasswordSent");
		}

		public ActionResult ResetPassword(string id)
		{
			GenerateRsaInformation();

			if (_repo.ValidateForgotPasswordHash(id))
				return View();
			else
				return Redirect("/");
		}

		[HttpPost]
		public ActionResult ResetPassword(string id, ResetPasswordModel model)
		{
			var rsaProvider = new RSACryptoServiceProvider();
			rsaProvider.FromXmlString(Session["Encryption"].ToString());
			string Password =
				Encoding.ASCII.GetString(rsaProvider.Decrypt(Convert.FromBase64String(model.NewPassword), false));

			string ConfirmPassword =
				Encoding.ASCII.GetString(rsaProvider.Decrypt(Convert.FromBase64String(model.ConfirmPassword), false));

			if (!Password.Equals(ConfirmPassword))
			{
				ModelState.AddModelError("NoMatch", "The password don't match. Please try again.");
				return View();
			}

			try
			{
				var user = _repo.FindByForgotPasswordHash(id);
				user.ExpireAllSessions();

				_repo.CompleteForgotPassword(Hash.HashPassword(Password), id);
				_uow.Commit();
				return View("ResetPasswordComplete");
			}
			catch (Exception)
			{
				return Redirect("/");
			}
		}

		private string UserIP
		{
			get
			{
				string header = Request.Headers["X-Forwarded-For"];
				if (!string.IsNullOrEmpty(header))
				{
					return Request.Headers["X-Forwarded-For"];
				}
				else
				{
					return Request.UserHostAddress;
				}
			}
		}

		[NonAction]
		private AuthenticationResponse AttemptLogin(string username, string password, out int? adminUserID)
		{
			adminUserID = null;

			// check user attempts to see if it should be locked out
			if (_attemptRepo.TestLockout(username, UserIP)) return AuthenticationResponse.LockedOut;

			// then check to see if they are in the local database
			var user = _repo.FindByEmailAddress(username);
			if (user != null)
			{
				var result = Crypto.VerifyHashedPassword(user.Password, password);

				if (result)
				{
					adminUserID = user.ID;
					return AuthenticationResponse.LocalSuccess;
				}
			}

			// then remote auth
			if (Remote.Authenticate(username, password, UserIP) == RemoteAuthenicationResponse.Valid) return AuthenticationResponse.ImagemakersSuccess;

			// all else fails, add the failed attempt and return unauthorized
			_attemptRepo.Add(username, UserIP);
			_uow.Commit();

			return AuthenticationResponse.Unauthorized;
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
