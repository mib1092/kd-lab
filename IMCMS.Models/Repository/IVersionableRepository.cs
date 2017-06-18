using IMCMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    public interface IVersionableRepository<T> : IRepository<T>
        where T : class, IVersionable, new()
    {
        /// <summary>
        /// Get past revisions of the object
        /// </summary>
        /// <param name="BaseID"></param>
        /// <param name="NumberOfRevisions"></param>
        /// <returns></returns>
        IEnumerable<T> GetVersions(int BaseID, int NumberOfRevisions = 10);
        T GetLive(int BaseID);
    }
}
