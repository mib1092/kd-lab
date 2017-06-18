using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IMCMS.Models.Entities
{
    public class TeamMember : Versionable
    {
        [Required]
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
        public string YearStarted { get; set; }
        public string Email { get; set; }

        public int? PhotoID { get; set; }
        public virtual Photo Photo { get; set; }

        public int Order { get; set; }

        [NotMapped]
        public string Fullname
        {
            get
            {
                return String.Format("{0} {1}", Firstname, Lastname);
            }
        }
    }
}
