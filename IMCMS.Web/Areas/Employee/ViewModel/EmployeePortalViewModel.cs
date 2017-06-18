using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using IMCMS.Models.Entities;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Employee.ViewModel
{
    
    public class EmployeePortalIndexViewModel : BaseViewModel
    {
        public EmployeePortalIndexViewModel()
        {
            Pages = new List<EmployeePortalPage>();
        }

        public string Headline { get; set; }
        public string Description { get; set; }

        public ICollection<EmployeePortalPage> Pages { get; set; }
    }
}