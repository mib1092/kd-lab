using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using Entities;

    public interface IAdminUserSessionRepository
    {
        bool IsGuidExpired(Guid guid);

        void Add(string username, string ip, Guid cookieGuid, int? adminUserID = null);

        void ExpireAllSessionsForUser(string username);

        IQueryable<AdminUserSession> GetAllForUserID(int id);

        IQueryable<AdminUserSession> GetAllForUsername(string username);
    }
}
