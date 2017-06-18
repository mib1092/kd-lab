using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMCMS.Models.Entities
{
    public class EmployeePortalPage : Versionable
    {
        [Required(ErrorMessage = "A title is required.")]

        public string Title { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }

        public PortalType PageType { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsNewWindow { get; set; }

        public int? ParentId { get; set; }
        public virtual EmployeePortalPage Parent { get; set; }

        public virtual ICollection<EmployeePortalPage> Children { get; set; }
        
    }

    public enum PortalType
    {
        Standard,
        Redirect
    }

}
