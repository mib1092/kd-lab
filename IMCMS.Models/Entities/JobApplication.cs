using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IMCMS.Models.Entities
{
    public class JobApplication
    {
        public JobApplication()
        {
            History = new List<JobHistory>();
        }

        public int ID { get; set; }
        public DateTime Submitted { get; set; }

        public ApplicationStatus Status { get; set; }
        
        public string Notes { get; set; }
        public DateTime? EmailSent { get; set;  }

        public int? JobID { get; set; }
        public virtual Job Job { get; set; }

        public string AppJobID { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public string DriversLicense { get; set;  }
        public string DriversLicenseState { get; set; }

        public bool? Felony { get; set; }
        public bool? USAuthorized { get; set; }
        public bool? IsEighteen { get; set; }

        public bool Agreement { get; set; }

        public virtual ICollection<JobHistory> History { get; set; }
    }

    public enum ApplicationStatus
    {
        New,
        Approved,
        Rejected,
        Archived,
        Prescreened,
        InConsideration
    }


}
