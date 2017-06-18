using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Entities
{
    public class JobHistory
    {
        public int ID { get; set; }

        public int? JobApplicationID { get; set; }
        public virtual JobApplication JobApp { get; set; }

        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }

        public string Description { get; set; }

    }
}
