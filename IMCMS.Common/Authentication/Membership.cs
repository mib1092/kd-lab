using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Security;
using System.Web;
using System.Security.Principal;
using IMCMS.Models;
using IMCMS.Models.Entities;
using AppHarbor.Web.Security;

namespace IMCMS.Common.Authentication
{
    public sealed class Membership
    {
        /// <summary>
        /// Check username and password against varying data sources. 
        /// The local database and the remote Imagemakers authentication service.
        /// This will create the authentication token
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static AuthenticationResponse VerifyUser(string Username, string Password, string IP = "")
        {
            AuthenticationResponse response = Local.ValidateUser(Username, Password, IP);
            if (response == AuthenticationResponse.Authorized)
            {
                CreateAuthenticationTicket(Username, false);
                return AuthenticationResponse.Authorized;
            }

            if (response == AuthenticationResponse.IPLocked)
            {
                Local.LogUnauthorizedAttempt(Username, IP);
                return AuthenticationResponse.IPLocked;
            }

            if (response == AuthenticationResponse.AccountLocked)
            {
                Local.LogUnauthorizedAttempt(Username, IP);
                return AuthenticationResponse.AccountLocked;
            }

            RemoteAuthenicationResponse RemoteAuth = Remote.Authenticate(Username, Password);

            if (RemoteAuth == RemoteAuthenicationResponse.Valid)
            {
                CreateAuthenticationTicket(Username, true);
                return AuthenticationResponse.Authorized;
            }
            else if(RemoteAuth == RemoteAuthenicationResponse.IPLocked) {
                return AuthenticationResponse.RemoteIPLocked;
            }
            else if (RemoteAuth == RemoteAuthenicationResponse.UnableToComplete ||
                RemoteAuth == RemoteAuthenicationResponse.NotConfigured)
            {
                response = Local.ValidateUser(Username, Password, IP, true);
                if (response == AuthenticationResponse.Authorized)
                {
                    return AuthenticationResponse.Authorized;
                }
            }

            Local.LogUnauthorizedAttempt(Username, IP);

            return AuthenticationResponse.Unauthorized;
        }

        public static void CreateUser(string Username, string Password)
        {
            Local.CreateUser(Username, Password, false);
        }

        public static void ForgotPassword(string Username)
        {
            Local.ForgotPassword(Username);
        }

        public static void ChangePassword(string Username, string OldPassword, string NewPassword)
        {
            Local.ChangePassword(Username, OldPassword, NewPassword);
        }

        /// <summary>
        /// Check password reset hash
        /// </summary>
        /// <param name="Hash"></param>
        /// <returns></returns>
        public static bool ForgotPasswordHashCheck(string Hash)
        {
            return Local.ForgotPasswordHashCheck(Hash);
        }

        public static void ForgotPasswordReset(string Password, string Hash)
        {
            Local.ForgotPasswordResetPassword(Password, Hash);
        }

        private static void CreateAuthenticationTicket(string Username, bool IsImagemakers) {
            IAuthenticator authenticator = new CookieAuthenticator();
            authenticator.SetCookie(Username, false, (IsImagemakers) ? new string[] { "IM" } : null);
        }

        public static bool IsImagemakersUser()
        {
            if (HttpContext.Current == null)
                return false;

            if (HttpContext.Current.User.Identity == null)
                return false;

            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return false;

            return HttpContext.Current.User.IsInRole("IM");
        }

        public static void PrepareLogin()
        {
            Remote.PingAuthentication();
        }

        public static bool VerifyDatabase()
        {
            return Local.VerifyDatabase();
        }

        public static User Initialize()
        {
            User u = new User { Username = "info@imagemakers-inc.com", Password = Utility.RandomString.Generate(40) };
            Local.CreateDatabaseTables();
            Local.CreateUser(u.Username, u.Password, true);
            return u;
        }
    }
}
