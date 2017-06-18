namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTeamMember : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Firstname = c.String(nullable: false),
                        Lastname = c.String(),
                        Department = c.String(),
                        JobTitle = c.String(),
                        YearStarted = c.String(),
                        Email = c.String(),
                        PhotoID = c.Int(),
                        Order = c.Int(nullable: false),
                        BaseID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Visbility = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Slug = c.String(),
                        Who = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Photos", t => t.PhotoID)
                .Index(t => t.PhotoID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TeamMembers", "PhotoID", "dbo.Photos");
            DropIndex("dbo.TeamMembers", new[] { "PhotoID" });
            DropTable("dbo.TeamMembers");
        }
    }
}
