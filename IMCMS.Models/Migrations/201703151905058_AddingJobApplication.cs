namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingJobApplication : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.JobApplications",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Submitted = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                        JobID = c.Int(),
                        AppJobID = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Address = c.String(),
                        City = c.String(),
                        State = c.String(),
                        ZipCode = c.String(),
                        DateOfBirth = c.DateTime(),
                        DriversLicense = c.String(),
                        DriversLicenseState = c.String(),
                        Felony = c.Boolean(),
                        USAuthorized = c.Boolean(),
                        Agreement = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Jobs", t => t.JobID)
                .Index(t => t.JobID);
            
            CreateTable(
                "dbo.JobHistories",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        JobApplicationID = c.Int(),
                        StartDate = c.String(),
                        EndDate = c.String(),
                        Company = c.String(),
                        Position = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.JobApplications", t => t.JobApplicationID)
                .Index(t => t.JobApplicationID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.JobApplications", "JobID", "dbo.Jobs");
            DropForeignKey("dbo.JobHistories", "JobApplicationID", "dbo.JobApplications");
            DropIndex("dbo.JobHistories", new[] { "JobApplicationID" });
            DropIndex("dbo.JobApplications", new[] { "JobID" });
            DropTable("dbo.JobHistories");
            DropTable("dbo.JobApplications");
        }
    }
}
