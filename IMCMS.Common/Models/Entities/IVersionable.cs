using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Entities
{
    public interface IVersionable
    {
        /// <summary>
        /// Key/identity column
        /// </summary>
        int ID { get; set; }

        /// <summary>
        /// Column that groups items together
        /// </summary>
        int BaseID { get; set; }

        /// <summary>
        /// The objects status, Live, History, etc
        /// </summary>
        VersionableItemStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the object visbility on the live site
        /// </summary>
        VersionableVisbility Visbility { get; set; }

        /// <summary>
        /// When this item was created
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// The URL stub (if used)
        /// </summary>
        string Slug { get; set; }
    }
}
