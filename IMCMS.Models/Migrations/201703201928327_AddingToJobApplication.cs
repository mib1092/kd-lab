namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingToJobApplication : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobApplications", "Notes", c => c.String());
            AddColumn("dbo.JobApplications", "EmailSent", c => c.DateTime());
            AddColumn("dbo.JobApplications", "Phone", c => c.String());
            AddColumn("dbo.JobApplications", "Email", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobApplications", "Email");
            DropColumn("dbo.JobApplications", "Phone");
            DropColumn("dbo.JobApplications", "EmailSent");
            DropColumn("dbo.JobApplications", "Notes");
        }
    }
}
