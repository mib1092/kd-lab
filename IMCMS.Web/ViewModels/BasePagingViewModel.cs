using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web.ViewModels
{
    public class BasePagingViewModel<T> : BaseViewModel<T>
    {
        public int CurrentPage { get; set; }

        public bool HasMoreItems { get; set; }
    }
}