using IMCMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    public abstract class VersionableRepository<T> : Repository<T>, IVersionableRepository<T>
        where T : class, IVersionable, new()
    {
        public VersionableRepository(IUnitOfWork UOW) : base(UOW)
        {
            
        }

        public IEnumerable<T> GetVersions(int BaseID, int NumberOfRevisions = 10)
        {
            return base.GetAll()
                .Where(d => d.Status == VersionableItemStatus.History && d.BaseID == BaseID)
                .OrderByDescending(d => d.Created)
                .Take(NumberOfRevisions);
        }

        public override void Add(T entity)
        {
            int NewID = (base.GetAll().Max(d => d.BaseID)) + 1;

            entity.BaseID = NewID;
            entity.Status = VersionableItemStatus.Live;
            base.Add(entity);
        }

        public override void Edit(T entity)
        {
            IQueryable<T> stuff = base.GetAll().Where(d => d.Status == VersionableItemStatus.Live && d.BaseID == entity.BaseID);
            foreach (T item in stuff)
            {
                item.Status = VersionableItemStatus.History;
            }
            entity.Status = VersionableItemStatus.Live;
            base.Edit(entity);
        }

        public override void Delete(T entity)
        {
            entity.Status = VersionableItemStatus.Trash;
        }


        public T GetLive(int BaseID)
        {
            return base.FindBy(d => d.Status == VersionableItemStatus.Live && d.BaseID == BaseID).FirstOrDefault();
        }
    }
}
