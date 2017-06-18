using IMCMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMCMS.Common.Extensions;

namespace IMCMS.Models.Repository
{
    using System.ComponentModel.DataAnnotations.Schema;

    using Common.Attributes;

    public class VersionableRepository<T> : Repository<T>, IVersionableRepository<T>
        where T : class, IVersionable, new()
    {
        public VersionableRepository(DbContext context)
            : base(context)
        {
        }

        public override IQueryable<T> GetAll()
        {
            return base.GetAll().Where(d => d.Status == VersionableItemStatus.Live);
        }

        public IEnumerable<T> GetVersions(int baseID, int numberOfRevisions = 10)
        {
            return base.GetAll()
                .Where(d => d.Status == VersionableItemStatus.History && d.BaseID == baseID)
                .OrderByDescending(d => d.Created)
                .Take(numberOfRevisions);
        }

        public override void Add(T entity)
        {
            Add(entity, null);
        }

        public void Add(T entity, int? baseId)
        {
            if (!baseId.HasValue)
            {
                int newID;
                if (base.GetAll().Any())
                {
                    newID = (base.GetAll().Max(d => d.BaseID)) + 1;
                }
                else
                {
                    newID = 1;
                }

                entity.BaseID = newID;
            }
            else
            {
                entity.BaseID = baseId.Value;
            }

            entity.Status = VersionableItemStatus.Live;
            base.Add(entity);
        }

        public override void Edit(T entity)
        {
            throw new ArgumentException("With a versioned object, use Edit(T, int) to version correctly.");
        }

        public void Edit(T entity, int baseID)
        {
            var live = GetLive(baseID);  // get the current live version so we can merge next

            //MergeWith(entity, live); // merge all properties from the live object into the submitted entity

            entity.BaseID = baseID;

            live.Status = VersionableItemStatus.History; // degrade the current live object to history item
            entity.Status = VersionableItemStatus.Live; // sanity check, ensure the new version is going to be live

            //_dbSet.Add(entity); // we're inserting a verion while keeping the old stuff
        }

        public override void Delete(T entity)
        {
            entity.Status = VersionableItemStatus.Trash;
        }

        public void Delete(int baseID)
        {
            var live = GetLive(baseID);
            live.Status = VersionableItemStatus.Trash;
        }

        public void Undelete(T entity)
        {
            entity.Status = VersionableItemStatus.Live;
        }

        public void Undelete(int baseID)
        {
            var obj = FindBy(d => d.BaseID == baseID && d.Status == VersionableItemStatus.Trash).First();
            obj.Status = VersionableItemStatus.Live;
        }


        public T GetLive(int baseID)
        {
            return FindBy(d => d.Status == VersionableItemStatus.Live && d.BaseID == baseID).FirstOrDefault();
        }
    }
}
