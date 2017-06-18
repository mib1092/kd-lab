using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace IMCMS.Models.Entities
{
    public abstract class Versionable : IVersionable
    {
        protected Versionable()
        {
            Created = DateTime.Now;

            // sanity check to ensure that we're not going to throw null exceptions randomly
            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity.IsAuthenticated)
                Who = HttpContext.Current.User.Identity.Name;
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int BaseID { get; set; }
        public VersionableItemStatus Status { get; set; }
        public VersionableVisbility Visbility { get; set; }
        public DateTime Created { get; set; }
        public string Slug { get; set; }
        public string Who { get; set; }
    }

    public enum VersionableItemStatus
    {
        Live,
        Draft,
        History,
        Trash
    }

    public enum VersionableVisbility
    {
        Public,
        Unlinked,
        Unpublished
    }
}
