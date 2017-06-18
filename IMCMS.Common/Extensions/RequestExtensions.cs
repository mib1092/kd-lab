using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using IMCMS.Common.Configuration;
using System.Text.RegularExpressions;
using System.Net;

namespace IMCMS.Common.Extensions
{
    public static class RequestExtensions
    {

        /// <summary>
        /// Determines out if the current request is a live server using the regex from web.config
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if running on a live server, false if not</returns>
        public static bool IsLive(this HttpRequestBase request)
        {
            foreach (UrlEntry regex in Config.ConfigurationSection.Urls.Live)
            {
                bool IsMatch = Regex.IsMatch(request.Url.Host, regex.Regex);
                if (IsMatch)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines out if the current request is a staging server using the regex from web.config
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if running on a staging server, false if not</returns>
        public static bool IsStaging(this HttpRequestBase request)
        {
            foreach (UrlEntry regex in Config.ConfigurationSection.Urls.Staging)
            {
                bool IsMatch = Regex.IsMatch(request.Url.Host, regex.Regex);
                if (IsMatch)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the user agent is a smartphone (no tablets)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if smartphone</returns>
        public static bool IsMobile(this HttpRequestBase request)
        {
            if (request.Cookies[Constants.EscapeNativeCookieName] != null)
            {
                if (request.Cookies[Constants.EscapeNativeCookieName].Value == Constants.MobileSiteValue) return true;
                if (request.Cookies[Constants.EscapeNativeCookieName].Value == Constants.FullSiteValue) return false;
            }

            string u = request.UserAgent;
            if (String.IsNullOrEmpty(u) || u.Length < 6)
                return false;

            Regex b = new Regex(@"android|avantgo|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\\/|plucker|pocket|psp|symbian|treo|up\\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\\-(n|u)|c55\\/|capi|ccwa|cdm\\-|cell|chtm|cldc|cmd\\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\\-s|devi|dica|dmob|do(c|p)o|ds(12|\\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\\-|_)|g1 u|g560|gene|gf\\-5|g\\-mo|go(\\.w|od)|gr(ad|un)|haie|hcit|hd\\-(m|p|t)|hei\\-|hi(pt|ta)|hp( i|ip)|hs\\-c|ht(c(\\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\\-(20|go|ma)|i230|iac( |\\-|\\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\\/)|klon|kpt |kwc\\-|kyo(c|k)|le(no|xi)|lg( g|\\/(k|l|u)|50|54|e\\-|e\\/|\\-[a-w])|libw|lynx|m1\\-w|m3ga|m50\\/|ma(te|ui|xo)|mc(01|21|ca)|m\\-cr|me(di|rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\\-2|po(ck|rt|se)|prox|psio|pt\\-g|qa\\-a|qc(07|12|21|32|60|\\-[2-7]|i\\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\\-|oo|p\\-)|sdk\\/|se(c(\\-|0|1)|47|mc|nd|ri)|sgh\\-|shar|sie(\\-|m)|sk\\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\\-|v\\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\\-|tdg\\-|tel(i|m)|tim\\-|t\\-mo|to(pl|sh)|ts(70|m\\-|m3|m5)|tx\\-9|up(\\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|xda(\\-|2|g)|yas\\-|your|zeto|zte\\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex a = new Regex(@"android", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex m = new Regex(@"mobile", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            bool t = false;
            if (a.IsMatch(u))
            {
                t = !m.IsMatch(u);
            }
            if ((b.IsMatch(u) || v.IsMatch(u.Substring(0, 4))) && !t)
            {
                return true;
            }
            return false;
        }


        public static bool IsLive(this HttpRequest request)
        {
            foreach (UrlEntry regex in Config.ConfigurationSection.Urls.Live)
            {
                bool IsMatch = Regex.IsMatch(request.Url.Host, regex.Regex);
                if (IsMatch)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines out if the current request is a staging server using the regex from web.config
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if running on a staging server, false if not</returns>
        public static bool IsStaging(this HttpRequest request)
        {
            foreach (UrlEntry regex in Config.ConfigurationSection.Urls.Staging)
            {
                bool IsMatch = Regex.IsMatch(request.Url.Host, regex.Regex);
                if (IsMatch)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Determines if the user agent is a smartphone (no tablets)
        /// </summary>
        /// <param name="request"></param>
        /// <returns>true if smartphone</returns>
        public static bool IsMobile(this HttpRequest request)
        {
            return IsMobile(new HttpRequestWrapper(request));
        }

        /// <summary>
	    /// method to get Client ip address
	    /// </summary>
	    /// <param name="request"></param>
	    /// <param name="getLan"> set to true if want to get local(LAN) Connected ip address</param>
	    /// <returns></returns>
	    public static string GetVisitorIPAddress(this HttpRequestBase request, bool getLan = false)
        {
            string visitorIPAddress = request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (String.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
                visitorIPAddress = request.UserHostAddress;

            if (string.IsNullOrEmpty(visitorIPAddress) || visitorIPAddress.Trim() == "::1")
            {
                getLan = true;
                visitorIPAddress = string.Empty;
            }

            if (getLan)
            {
                if (string.IsNullOrEmpty(visitorIPAddress))
                {
                    //This is for Local(LAN) Connected ID Address
                    string stringHostName = Dns.GetHostName();
                    //Get Ip Host Entry
                    IPHostEntry ipHostEntries = Dns.GetHostEntry(stringHostName);
                    //Get Ip Address From The Ip Host Entry Address List
                    IPAddress[] arrIpAddress = ipHostEntries.AddressList;

                    try
                    {
                        visitorIPAddress = arrIpAddress[arrIpAddress.Length - 2].ToString();
                    }
                    catch
                    {
                        try
                        {
                            visitorIPAddress = arrIpAddress[0].ToString();
                        }
                        catch
                        {
                            try
                            {
                                arrIpAddress = Dns.GetHostAddresses(stringHostName);
                                visitorIPAddress = arrIpAddress[0].ToString();
                            }
                            catch
                            {
                                visitorIPAddress = "127.0.0.1";
                            }
                        }
                    }
                }
            }
            return visitorIPAddress;
        }
    }
}
