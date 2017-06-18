using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class GoogleAnalyticsElement : ConfigurationElement
    {
        [ConfigurationProperty("Enabled", DefaultValue = true, IsRequired = false)]
        public bool Enabled
        {
            get { return (bool)this["Enabled"]; }
        }

        [ConfigurationProperty("ID", DefaultValue = "", IsRequired = false)]
        public string ID
        {
            get { return (string)this["ID"]; }
        }

        [ConfigurationProperty("TrackOnStaging", DefaultValue = false, IsRequired = false)]
        public bool TrackOnStaging
        {
            get { return (bool)this["TrackOnStaging"]; }
        }

        [ConfigurationProperty("DomainName", DefaultValue = "", IsRequired = false)]
        public string DomainName
        {
            get { return (string)this["DomainName"]; }
        }



    }
}
