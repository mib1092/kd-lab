using System;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace IMCMS.Web
{
    public class ReverseProxySupportedRequireSslAttribute : RequireHttpsAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var request = filterContext.RequestContext.HttpContext.Request;

            if (!request.IsSecureConnection && !IsForwardedSsl(request) && !request.IsLocal)
            {
                // only redirect for GET requests, otherwise the browser might not propagate the verb and request
                // body correctly.

                if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("must use ssl");
                }

                string url = request.Url.ToString().Replace("http:", "https:");
                filterContext.Result = new RedirectResult(url, true);
            }
        }

        static bool IsForwardedSsl(HttpRequestBase request)
        {
            var xForwardedProto = request.Headers.Get("X-Forwarded-Proto");
            var forwardedSsl = xForwardedProto != null && xForwardedProto.IndexOf("https", StringComparison.OrdinalIgnoreCase) >= 0;
            return forwardedSsl;
        }
    }
}