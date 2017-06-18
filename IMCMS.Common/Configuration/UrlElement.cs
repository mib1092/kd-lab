using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class UrlElement : ConfigurationElement
    {
        [ConfigurationProperty("Staging", IsDefaultCollection = false)]
        public UrlCollection Staging
        {
            get { return (UrlCollection)this["Staging"]; }
        }

        [ConfigurationProperty("Live", IsDefaultCollection = false)]
        public UrlCollection Live
        {
            get { return (UrlCollection)this["Live"]; }
        }
    }
}
