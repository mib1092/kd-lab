using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using System.Data.Entity;

    using Entities;

    public class AdminUserSessionRepository : Repository<AdminUserSession>, IAdminUserSessionRepository
    {
        public AdminUserSessionRepository(DbContext context)
            : base(context)
        {

        }

        public bool IsGuidExpired(Guid guid)
        {
            return _dbSet.Any(x => x.CookieGuid == guid && x.IsExpired);
        }


        public void Add(string username, string ip, Guid cookieGuid, int? userID = null)
        {
            _dbSet.Add(new AdminUserSession { Username = username, CookieGuid = cookieGuid, IP = ip, AdminUserID = userID});
        }

        public void ExpireAllSessionsForUser(string username)
        {
            var sessions = _dbSet.Where(x => x.Username == username);

            foreach (var session in sessions)
            {
                session.IsExpired = true;
            }
        }

        public IQueryable<AdminUserSession> GetAllForUserID(int id)
        {
            return _dbSet.Where(x => x.AdminUserID == id);
        }

        public IQueryable<AdminUserSession> GetAllForUsername(string username)
        {
            return _dbSet.Where(x => x.Username == username);
        }
    }
}
