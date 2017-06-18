using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using IMCMS.Web.ViewModels;

namespace IMCMS.Web.Areas.Employee.ViewModel
{
    public class LoginModel : BaseViewModel
    {
        [Required]
        public string Password { get; set; }
    }
}