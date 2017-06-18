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

        /// <summary>
        /// Get the live version of the object
        /// </summary>
        /// <param name="BaseID"></param>
        /// <returns></returns>
        T GetLive(int BaseID);

        /// <summary>
        /// Gets all items with their live version
        /// </summary>
        /// <returns></returns>
        new IQueryable<T> GetAll();

        /// <summary>
        /// Add a new versionable item to the database. This will automatically become the live version.
        /// </summary>
        /// <param name="entity"></param>
        new void Add(T entity);

        /// <summary>
        /// Add a new versionable item to the database. This will automatically become the live version.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="baseId"></param>
        new void Add(T entity, int? baseId);

        /// <summary>
        /// Update/edit/modify an versionable item in the database. This entity will become the live version.
        /// </summary>
        /// <param name="entity"></param>
        new void Edit(T entity);

        /// <summary>
        /// Update/edit/modify an versionable item in the database. This entity will become the live version.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="BaseID">Base ID of object to be edited</param>
        void Edit(T entity, int BaseID);

        /// <summary>
        /// Delete/hide a versionable item in the database. 
        /// Status is updated to prevent it from rendering, but never explicitly deleted from the database for recovery purposes
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        new void Delete(T entity);

        /// <summary>
        /// Delete/hide a versionable item in the database. 
        /// Status is updated to prevent it from rendering, but never explicitly deleted from the database for recovery purposes
        /// </summary>
        /// <param name="BaseID">The BaseID of the object</param>
        void Delete(int BaseID);

        /// <summary>
        /// Restore/undelete a versionable item in the database.
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        void Undelete(T entity);

        /// <summary>
        /// Restore/undelete a versionable item in the database. 
        /// </summary>
        /// <param name="BaseID">The BaseID of the object</param>
        void Undelete(int BaseID);
    }
}
