using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMCMS.Models.Entities;

namespace IMCMS.Models.Repository
{
    public interface IPortalSettingsPageRepository : IVersionableRepository<EmployeePortalPage>
    {
        EmployeePortalPage GetPageBySlug(string slug);
    }
}
