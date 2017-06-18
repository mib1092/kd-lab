using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using Entities;

    public interface IAdminUserRepository : IRepository<AdminUser>
    {
        AdminUser FindByEmailAddress(string emailAddress);

        bool IsEmailUnique(string emailAddress);

        string GenerateForgotPassword(string emailAddress);

        bool ValidateForgotPasswordHash(string forgotPasswordHash);

        AdminUser FindByForgotPasswordHash(string forgotPasswordHash);

        void CompleteForgotPassword(string newHashedPassword, string forgotPasswordHash);

        AdminUser FindById(int id);
    }
}
