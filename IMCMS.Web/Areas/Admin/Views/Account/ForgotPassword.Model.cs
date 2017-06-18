using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMCMS.Web.Areas.Admin.Views.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ForgotPasswordModel
    {
        [Required]
        [EmailAddress]
        public string EmailAddress { get; set; }
    }
}