namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveDOBfromJobApp : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.JobApplications", "DateOfBirth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.JobApplications", "DateOfBirth", c => c.DateTime());
        }
    }
}
