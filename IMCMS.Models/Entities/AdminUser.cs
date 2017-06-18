using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IMCMS.Models.Entities
{

	public class AdminUser
    {
        public AdminUser()
        {
            Roles = new List<AdminRole>();
            Sessions = new List<AdminUserSession>();
        }

        public int ID { get; set; }

        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }

        public string PasswordResetHash { get; set; }

        public DateTime? PasswordResetTime { get; set; }

        public virtual ICollection<AdminRole> Roles { get; set; }

        public virtual ICollection<AdminUserSession> Sessions { get; set; }

        public bool Disabled { get; set; }

        public void ExpireAllSessions()
        {
            foreach (var session in Sessions)
            {
                session.IsExpired = true;
            }
        }
    }
}
