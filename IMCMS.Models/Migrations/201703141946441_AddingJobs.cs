namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingJobs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Jobs",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        City = c.String(nullable: false),
                        State = c.String(nullable: false),
                        Specs = c.String(),
                        JobID = c.String(),
                        JobType = c.Int(),
                        Summary = c.String(),
                        Description = c.String(),
                        ListOnWebsite = c.Boolean(nullable: false),
                        ListOnIndeed = c.Boolean(nullable: false),
                        Order = c.Int(nullable: false),
                        IndeedDate = c.DateTime(),
                        IndeedRef = c.String(),
                        PostalCode = c.String(),
                        Hours = c.String(),
                        Wage = c.String(),
                        Education = c.String(),
                        Category = c.String(),
                        Experience = c.String(),
                        BaseID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Visbility = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Slug = c.String(),
                        Who = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Jobs");
        }
    }
}
