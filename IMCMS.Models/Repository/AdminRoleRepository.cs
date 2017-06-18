using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using System.Data.Entity;

    using Entities;

    public class AdminRoleRepository : Repository<AdminRole>, IAdminRoleRepository
    {
        public AdminRoleRepository(DbContext context)
            : base(context)
        {

        }

        public AdminRole FindByID(int id)
        {
            return _dbSet.FirstOrDefault(x => x.ID == id);
        }
    }
}
