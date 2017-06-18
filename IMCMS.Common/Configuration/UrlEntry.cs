using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class UrlEntry : ConfigurationElement {
        [ConfigurationProperty("Regex", IsKey = false, IsRequired = true)]
       public String Regex {
          get { return (String)base["Regex"]; }
          set { base["Regex"] = value; }
       }

        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public String Name
        {
            get { return (String)base["Name"]; }
            set { base["Name"] = value; }
        }
    }
}
