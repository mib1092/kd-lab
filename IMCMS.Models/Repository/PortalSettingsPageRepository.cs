using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using IMCMS.Models.Entities;

namespace IMCMS.Models.Repository
{
    public class PortalSettingsPageRepository : VersionableRepository<EmployeePortalPage>, IPortalSettingsPageRepository
    {
        public PortalSettingsPageRepository(DbContext context) : base(context)
        {

        }

        public EmployeePortalPage GetPageBySlug(string slug)
        {
            return base.GetAll().FirstOrDefault(x => x.Slug == slug && x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public);
        }
    }
}
