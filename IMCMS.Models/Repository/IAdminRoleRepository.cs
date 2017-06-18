namespace IMCMS.Models.Repository
{
    using Entities;

    public interface IAdminRoleRepository : IRepository<AdminRole>
    {
        AdminRole FindByID(int id);
    }
}
