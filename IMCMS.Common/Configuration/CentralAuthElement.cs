using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace IMCMS.Common.Configuration
{
    public class CentralAuthElement : ConfigurationElement
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

        [ConfigurationProperty("Secret", DefaultValue = "", IsRequired = false)]
        public string Secret
        {
            get { return (string)this["Secret"]; }
        }
    }
}
