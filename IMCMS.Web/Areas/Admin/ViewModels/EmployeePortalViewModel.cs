using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using IMCMS.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace IMCMS.Web.Areas.Admin.ViewModels
{
    public class EmployeePortalViewModel : AdminBaseViewModel<EmployeePortalSettings>
    {
        public EmployeePortalViewModel()
        {
            Pages = new List<EmployeePortalPage>();
        }

        public List<EmployeePortalPage> Pages;
    }
}