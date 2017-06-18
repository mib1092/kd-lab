using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using IMCMS.Models.Entities;

namespace IMCMS.Models.Repository
{
    public class JobRepository : VersionableRepository<Job>
    {
        public JobRepository(DbContext context) : base(context)
        {

        }

        public List<Job> GetIndeedJobs()
        {
            return GetAll().Where(x => !x.IsDefault && x.ListOnIndeed && x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public).ToList();
        }

        public List<Job> GetAllPublic()
        {
            return GetAll().Where(x => !x.IsDefault && x.ListOnWebsite && x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public).ToList();
        }

        public Job GetBySlug(string slug)
        {
            return GetAll().FirstOrDefault(x => !x.IsDefault && x.Slug == slug && x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public);
        }
    }
}
