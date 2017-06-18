using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using IMCMS.Models.Entities;

namespace IMCMS.Web.ViewModels
{
    public class CareersViewModel : BaseViewModel
    {
        public CareersViewModel()
        {
            Jobs = new List<Job>();
            JobList = new List<SelectListItem>();
            IsApplyView = false;
        }

        public bool IsApplyView { get; set; }
        public ICollection<Job> Jobs { get; set; }
        public JobApplication JobApp { get; set; }
        public List<SelectListItem> JobList { get; set; }
    }
}