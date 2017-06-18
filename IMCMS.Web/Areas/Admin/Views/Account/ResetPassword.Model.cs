namespace IMCMS.Web.Areas.Admin.Views.Account
{
    using System.ComponentModel.DataAnnotations;

    public class ResetPasswordModel
    {
        public string NewPassword { get; set; }

        // cannot use [Compare] here since the views use javascript encryption preventing these to equal, without decryption first
        public string ConfirmPassword { get; set; }
    }
}