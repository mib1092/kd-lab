using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IMCMS.Models.Entities
{
    public class Job : Versionable
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        public string Specs { get; set; }
        public string JobID { get; set; }
        public JobType? JobType { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }

        public bool ListOnWebsite { get; set; }
        public bool ListOnIndeed { get; set; }

        public int Order { get; set; }

        /* Fields for Indeed posting */
        public DateTime? IndeedDate { get; set; }
        public string IndeedRef { get; set; }
        public string PostalCode { get; set; }
        public string Hours { get; set; }
        public string Wage { get; set; }
        public string Education { get; set; }
        public string Category { get; set; }
        public string Experience { get; set; }

        public bool IsDefault { get; set; }

        public virtual ICollection<JobApplication> Applications { get; set; }
    }

    public enum JobType
    {
        Driving,
        Non_Driving
    }
}
