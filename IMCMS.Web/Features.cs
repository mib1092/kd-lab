using IMCMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace IMCMS.Web
{
    public static class Features
    {

        //public static Conditions MyFeature => Conditions.Everyone | Conditions.None;

        public static bool IsAvailable(Conditions condition)
        {
            var context = HttpContext.Current;
            if (condition.HasFlag(Conditions.LocalEnvironment) && context.Request.IsLocal)
                return true;

            if (condition.HasFlag(Conditions.ImagemakersUser) && context.User.IsInRole("IM"))
                return true;

            if (condition.HasFlag(Conditions.ImagemakersDeveloper) && context.User.IsInRole("IM") && context.Request.Cookies[Constants.DeveloperCookieName] != null)
                return true;

            if (condition.HasFlag(Conditions.AnyAdminUser) && context.User.Identity.IsAuthenticated)
                return true;

            if (condition.HasFlag(Conditions.Everyone))
                return true;

            return false;
        }

        public static Dictionary<string, Conditions> GetFlags()
        {
            return MethodBase.GetCurrentMethod().DeclaringType.GetProperties(BindingFlags.Public | BindingFlags.Static).ToDictionary(x => x.Name, x=> (Conditions)x.GetValue(null, null));
        }
    }

    [Flags]
    public enum Conditions
    {
        Everyone = 1,
        LocalEnvironment = 2,
        ImagemakersUser = 4,
        AnyAdminUser = 8,
        None = 16,
        ImagemakersDeveloper = 32
    }
}