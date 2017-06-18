using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    public interface IAdminUserAttemptRepository
    {
        void Add(string username, string ipAddress);

        bool TestLockout(string username, string ipAddresss);
    }
}
