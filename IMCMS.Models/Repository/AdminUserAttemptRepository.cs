using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using System.Data.Entity;

    using Entities;

    public class AdminUserAttemptRepository : IAdminUserAttemptRepository
    {
        protected DbContext _context;
        public DbSet<AdminUserAttempt> _dbSet;

        public AdminUserAttemptRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<AdminUserAttempt>();
        }


        public void Add(string username, string ipAddress)
        {
            _dbSet.Add(
                new AdminUserAttempt { IPAddress = ipAddress, Username = username });
        }

        public bool TestLockout(string username, string ipAddress)
        {
            var time = DateTime.Now.AddMinutes(-5);
            var items = _dbSet.Where(x => x.When > time  && x.When < DateTime.Now).ToList();

            return items.Count(x => x.IPAddress == ipAddress) > 20 || items.Count(x => x.Username == username) > 8;
        }
    }
}
