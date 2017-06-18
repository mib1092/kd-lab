using IMCMS.Models.DAL;
using IMCMS.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Services
{
    public class VersionableObjectService<T> where T : class, IVersionable
    {
        protected readonly DataContext context;
        public VersionableObjectService(IUnitOfWork uow) : this((DataContext)uow)
        {
            
        }

        public VersionableObjectService(DataContext db)
        {
            context = db;
        }

        public static VersionableResult<T> GenerateVersionableResult(T obj, bool canSeeUnpublished)
        {
            return GenerateVersionableResult(new DataContext(), obj, canSeeUnpublished);
        }

        public static VersionableResult<T> GenerateVersionableResult(DataContext db, T obj, bool canSeeUnpublished)
        {
            if (obj == null || obj.Status == VersionableItemStatus.Trash)
                return new VersionableResult<T> { Status = ResultStatus.NotFound };

            if (obj.Status == VersionableItemStatus.History)
            {
                var live = GetLive(db, obj.BaseID);
                return new VersionableResult<T> { Status = ResultStatus.MustRedirect, Object = live };
            }


            if (obj.Visbility == VersionableVisbility.Unpublished && canSeeUnpublished)
                return new VersionableResult<T> { Status = ResultStatus.UnpublishedButAllowed, Object = obj };

            if (obj.Visbility == VersionableVisbility.Unpublished && !canSeeUnpublished)
                return new VersionableResult<T> { Status = ResultStatus.NotFound };

            return new VersionableResult<T> { Status = ResultStatus.Ok, Object = obj };
        }

        protected virtual IQueryable<T> Queryable()
        {
            return context.Set<T>();
        }

        protected virtual IQueryable<T> QueryableAll()
        {
            return Queryable().Where(x => x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public).OrderBy(x => x.Created);
        }

        public virtual IEnumerable<T> GetAll()
        {
            return QueryableAll().ToList();
        }

        public async virtual Task<IEnumerable<T>> GetAllAsync()
        {
            return await QueryableAll().ToListAsync();
        }

        public virtual IEnumerable<T> GetAll(int perPage, int pageNumber)
        {
            return QueryableAll().Skip((pageNumber - 1) * (perPage)).Take(perPage + 1).ToList();   // cheat here to determine if there's more items
        }

        public virtual VersionableResult<T> GetObject(string slug, bool canSeeUnpublished)
        {
            var obj = Queryable().OrderBy(x => x.Status).FirstOrDefault(x => x.Slug == slug);
            return GenerateVersionableResult(context, obj, canSeeUnpublished);
        }

        static T GetLive(DataContext context, int baseID)
        {
            return context.Set<T>().Where(x => x.Status == VersionableItemStatus.Live && x.Visbility == VersionableVisbility.Public).FirstOrDefault(x => x.BaseID == baseID);
        }

        public async virtual Task<VersionableResult<T>> GetObjectAsync(string slug, bool canSeeUnpublished)
        {
            var obj = await Queryable().OrderBy(x => x.Status).FirstOrDefaultAsync(x => x.Slug == slug);
            return GenerateVersionableResult(context, obj, canSeeUnpublished);
        }
    }

    public class VersionableResult<T>
    {
        public T Object { get; set; }
        public ResultStatus Status { get; set; }
    }

    public enum ResultStatus
    {
        Ok,
        UnpublishedButAllowed,
        MustRedirect,
        NotFound
    }
}
