using System.Collections.Generic;

namespace IMCMS.Models.Entities
{
    public class AdminRole
    {
        public AdminRole()
        {
            Users = new List<AdminUser>();
        }

        public int ID { get; set; }
        
        public string Name { get; set; }

        public virtual ICollection<AdminUser> Users { get; set; } 
    }
}
