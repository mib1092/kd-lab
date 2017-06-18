using System;
using System.Text;
using System.Web.Mvc;
using System.Web;
using IMCMS.Common.Configuration;
using IMCMS.Common.Extensions;
using IMCMS.Common.Authentication;

namespace IMCMS.Common.Helpers
{
    public static class GoogleAnalyticsHelper
    {
        private static GoogleAnalyticsElement AnalyticsSettings = Config.ConfigurationSection.GoogleAnalytics;

        /// <summary>
        /// Generate Google Analytics tracking code. Profile ID read from web.config
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static MvcHtmlString GoogleAnalytics(this HtmlHelper helper)
        {
            return GoogleAnalytics(helper, AnalyticsSettings.ID);
        }

        /// <summary>
        /// Generate Google Analytics tracking code
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="ID">Google Analytics Profile ID</param>
        /// <returns></returns>
        public static MvcHtmlString GoogleAnalytics(this HtmlHelper helper, string ID)
        {
            HttpContextBase context = helper.ViewContext.HttpContext;

            if (AnalyticsSettings.Enabled)
            {
                if (!HttpContext.Current.Request.IsLocal)
                {
                    if (!context.Request.IsStaging() || (context.Request.IsStaging() && AnalyticsSettings.TrackOnStaging))
                    {
                        return String.IsNullOrEmpty(AnalyticsSettings.ID)
                            ? MvcHtmlString.Create("<p style=\"color:#f00 !important; font-weight: bold !important;\">Google Analytics tracking code missing</p>" + GetAnalyticsString(false, context))
                            : MvcHtmlString.Create(GetAnalyticsString(true, context));
                    }

                    return MvcHtmlString.Create("<!-- Google Analytics disabled on staging. Add/Change TrackOnStaging attribute in web.config -->" + GetAnalyticsString(false, context));
                }

                return MvcHtmlString.Create("<!-- Running locally, Google Analytics tracking disabled -->" + GetAnalyticsString(false, context));
            }

            return MvcHtmlString.Create(String.Empty);
        }

        private static string GetAnalyticsString(bool isEnabled, HttpContextBase context)
        {
            var sb = new StringBuilder();

            sb.Append("<script>");
            sb.Append("(function(i,s,o,g,r,a,m){i['GoogleAnalyticsObject']=r;i[r]=i[r]||function(){(i[r].q=i[r].q||[]).push(arguments)},i[r].l=1*new Date();a=s.createElement(o),m=s.getElementsByTagName(o)[0];a.async=1;a.src=g;m.parentNode.insertBefore(a,m)})(window,document,'script','//www.google-analytics.com/analytics.js','ga');");

            if (isEnabled)
            {
                sb.Append("ga('create', '" + AnalyticsSettings.ID + "', 'auto');");

                if (context.User.Identity.IsAuthenticated)
                    sb.Append("ga('set', 'IsLoggedIn', '" + (Membership.IsImagemakersUser() ? "Imagemakers" : "Admin") + "');");

                sb.Append("ga('send', 'pageview');");
            }

            sb.Append("</script>");

            return sb.ToString();
        }
    }

}
