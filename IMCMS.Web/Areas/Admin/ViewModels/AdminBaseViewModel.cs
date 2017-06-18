using IMCMS.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web.Areas.Admin.ViewModels
{
    public class AdminBaseViewModel : BaseViewModel
    {
        /// <summary>
        /// Gets or sets the value where the adminstrator can view the end user interface of their changes
        /// </summary>
        public string ViewPage { get; set; }

        /// <summary>
        /// Gets or sets a property for the URL which is the listing of the current object
        /// </summary>
        public string ListPage { get; set; }

        /// <summary>
        /// Gets or sets if the success message should be displyed
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets an error message
        /// </summary>
        public string Error { get; set; }
        
        /// <summary>
        /// Gets or sets if the current object is in a rollback condition
        /// </summary>
        public bool IsRollback { get; set; }
    }

    public class AdminBaseViewModel<T> : AdminBaseViewModel
    {
        /// <summary>
        /// The item you're displaying
        /// </summary>
        public T Item { get; set; }
    }
}