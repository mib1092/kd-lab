using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Entities
{
    public class AdminUserAttempt
    {
        public AdminUserAttempt()
        {
            When = DateTime.Now;
        }
        public int ID { get; set; }
        public string Username { get; set; }
        public string IPAddress { get; set; }
        public DateTime When { get; set; }
    }
}
