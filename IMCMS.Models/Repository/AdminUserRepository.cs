using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMCMS.Models.Repository
{
    using System.Data.Entity;

    using Common.Utility;

    using Entities;

    public class AdminUserRepository : Repository<AdminUser>, IAdminUserRepository
    {

        public AdminUserRepository(DbContext context)
            : base(context)
        {

        }

        public AdminUser FindByEmailAddress(string emailAddress)
        {
            return _dbSet.FirstOrDefault(x => !x.Disabled && x.EmailAddress == emailAddress);
        }

        public AdminUser FindByForgotPasswordHash(string forgotPasswordHash)
        {
            return _dbSet.FirstOrDefault(x => !x.Disabled && x.PasswordResetHash == forgotPasswordHash);
        }

        public AdminUser FindById(int id)
        {
            return _dbSet.FirstOrDefault(x => x.ID == id);
        }

        public bool IsEmailUnique(string emailAddress)
        {
            return !_dbSet.Any(x => x.EmailAddress == emailAddress);
        }

        public string GenerateForgotPassword(string emailAddress)
        {
            var hash = RandomString.Generate(24);
            var user = FindByEmailAddress(emailAddress);
            user.PasswordResetTime = DateTime.Now;
            user.PasswordResetHash = hash;

            return hash;
        }

        public bool ValidateForgotPasswordHash(string forgotPasswordHash)
        {
            var user = FindByForgotPasswordHash(forgotPasswordHash);

            if (user != null && user.PasswordResetTime.HasValue)
            {
                return user.PasswordResetTime.Value.AddHours(24) > DateTime.Now;
            }

            return false;
        }

        public void CompleteForgotPassword(string newHashedPassword, string forgotPasswordHash)
        {
            if (!ValidateForgotPasswordHash(forgotPasswordHash)) return;

            var user = FindByForgotPasswordHash(forgotPasswordHash);

            user.PasswordResetHash = null;
            user.PasswordResetTime = null;

            user.Password = newHashedPassword;
        }

        void IRepository<AdminUser>.Edit(AdminUser entity)
        {
            var user = FindById(entity.ID);
            _context.Entry(user).CurrentValues.SetValues(entity);
        }
    }
}
