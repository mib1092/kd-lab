using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Entities
{
    public class AdminUserSession
    {
        public AdminUserSession()
        {
            Created = DateTime.Now;
        }
        public int ID { get; set; }
        public string Username { get; set; }
        public string IP { get; set; }
        public bool IsExpired { get; set; }
        public Guid CookieGuid { get; set; }
        public DateTime Created { get; set; }

        public virtual AdminUser AdminUser { get; set; }
        public int? AdminUserID { get; set; }
    }
}
