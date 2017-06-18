namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingToJobApplication1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.JobApplications", "IsEighteen", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.JobApplications", "IsEighteen");
        }
    }
}
