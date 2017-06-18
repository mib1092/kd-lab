using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class ImagemakersSection : ConfigurationSection
    {
        [ConfigurationProperty("GoogleAnalytics", IsRequired = false)]
        public GoogleAnalyticsElement GoogleAnalytics
        {
            get { return (GoogleAnalyticsElement)base["GoogleAnalytics"]; }
        }


        [ConfigurationProperty("Urls", IsDefaultCollection = false)]
        public UrlElement Urls
        {
            get { return (UrlElement)base["Urls"]; }
        }

        [ConfigurationProperty("CentralAuth", IsRequired = false)]
        public CentralAuthElement CentralAuth
        {
            get { return (CentralAuthElement)base["CentralAuth"]; }
        }

        [ConfigurationProperty("Customer", IsRequired = true)]
        public CustomerElement Customer
        {
            get { return (CustomerElement)base["Customer"]; }
        }

    }
}
