using IMCMS.Models.Services;
using IMCMS.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web.ViewModels
{
    public class VersionableObjectViewModel<T> : BaseViewModel<T>
    {
        public ResultStatus Status { get; set; }
    }
}