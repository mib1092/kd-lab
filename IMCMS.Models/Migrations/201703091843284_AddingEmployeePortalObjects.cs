namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingEmployeePortalObjects : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmployeePortalPages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false),
                        Content = c.String(),
                        Order = c.Int(nullable: false),
                        PageType = c.Int(nullable: false),
                        RedirectUrl = c.String(),
                        IsNewWindow = c.Boolean(nullable: false),
                        ParentId = c.Int(),
                        BaseID = c.Int(nullable: false),
                        Status = c.Int(nullable: false),
                        Visbility = c.Int(nullable: false),
                        Created = c.DateTime(nullable: false),
                        Slug = c.String(),
                        Who = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.EmployeePortalPages", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.EmployeePortalSettings",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Password = c.String(nullable: false),
                        Headline = c.String(),
                        Description = c.String(),
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
            DropForeignKey("dbo.EmployeePortalPages", "ParentId", "dbo.EmployeePortalPages");
            DropIndex("dbo.EmployeePortalPages", new[] { "ParentId" });
            DropTable("dbo.EmployeePortalSettings");
            DropTable("dbo.EmployeePortalPages");
        }
    }
}
