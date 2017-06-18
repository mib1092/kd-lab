using System;
using System.Linq;

namespace IMCMS.Models.Repository
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Get all items in the database of type T
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAll();

        /// <summary>
        /// Gets items that match the where clause
        /// </summary>
        /// <param name="predicate">Where clause</param>
        /// <returns></returns>
        IQueryable<T> FindBy(System.Linq.Expressions.Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Adds/inserts an item to the database with added meta information
        /// </summary>
        /// <param name="entity"></param>
        void Add(T entity);

        /// <summary>
        /// Delete/hide a item in the database. 
        /// Status is updated to prevent it from rendering, but never explicitly deleted from the database for recovery purposes
        /// </summary>
        /// <param name="entity"></param>
        void Delete(T entity);


        /// <summary>
        /// Update/edit/modify an item in the database. 
        /// </summary>
        /// <param name="entity"></param>
        void Edit(T entity);
    }
}
