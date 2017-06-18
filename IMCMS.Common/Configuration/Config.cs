using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace IMCMS.Common.Configuration
{
    public class Config
    {
        public static ImagemakersSection ConfigurationSection
        {
            get
            {
                ImagemakersSection config;
                config = (ImagemakersSection)WebConfigurationManager.GetSection(
                   "Imagemakers");

                return config;
            }
        }
        public static string GetHostBaseUrl(Uri url)
        {
            //Default take from web.config
            string hostBaseUrl = ConfigurationManager.AppSettings["HostBaseUrl"];
            if (!String.IsNullOrEmpty(hostBaseUrl))
                return hostBaseUrl;

            //Otherwise, use current Request.Url to determine host url
            if (url == null) throw new ArgumentNullException("url");

            return String.Format("http://{0}/", url.Authority);
        }
    }
}
