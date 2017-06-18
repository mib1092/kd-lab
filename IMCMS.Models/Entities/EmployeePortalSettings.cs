using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IMCMS.Models.Entities
{
    public class EmployeePortalSettings : Versionable
    {
        [Required]
        public string Password { get; set; }

        public string Headline { get; set; }
        public string Description { get; set; }
    }
}
