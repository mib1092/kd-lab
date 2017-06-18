using System.Data.Entity;
using IMCMS.Common.Database;
using IMCMS.Models.Entities;

namespace IMCMS.Models.DAL
{
    public interface IUnitOfWork
    {
        DbSet<AdminUser> AdminUsers { get; set; }
        DbSet<AdminRole> AdminRoles { get; set; }
        DbSet<AdminUserAttempt> AdminUserAttempts { get; set; }
        DbSet<AdminUserSession> AdminUserSessions { get; set; }
        DbSet<Photo> Photos { get; set; }
        DbSet<EmployeePortalSettings> EmployeePortalSettings { get; set; }
        DbSet<EmployeePortalPage> EmployeePortalPages { get; set; }
        DbSet<TeamMember> TeamMembers { get; set; }
        DbSet<Job> Jobs { get; set; }
        DbSet<JobApplication> JobApplications { get; set; }
        DbSet<JobHistory> JobHistory { get; set; }
        DbSet<Contact> Contacts { get; set; }
        void Commit();
    }

    public class DataContext : DbContext, IUnitOfWork
    {

        /// <summary>
        /// Create new data context using the appropriate connection string for the location running. Local, Staging or Live
        /// </summary>
        public DataContext() : base(ConnectionString.GetConnectionTypeByServer()) { }

        /// <summary>
        /// Create a new data context using the connection string name
        /// </summary>
		/// <param name="connectionStringName">Connection string name as defined in the web.config</param>
        public DataContext(string connectionStringName) : base(connectionStringName) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Triggers a migration when the model is being created
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<DataContext, IMCMS.Models.Migrations.Configuration>());

            base.OnModelCreating(modelBuilder);
           
        }

        public DbSet<AdminUser> AdminUsers { get; set; }
        public DbSet<AdminRole> AdminRoles { get; set; }
        public DbSet<AdminUserAttempt> AdminUserAttempts { get; set; }
        public DbSet<AdminUserSession> AdminUserSessions { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<EmployeePortalSettings> EmployeePortalSettings { get; set; }
        public DbSet<EmployeePortalPage> EmployeePortalPages { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<JobHistory> JobHistory { get; set; }
        public DbSet<Contact> Contacts { get; set; }

        public void Commit()
        {
            base.SaveChanges();
        }
    }
}
