namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserStuff : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AdminRoles",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AdminUsers",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        EmailAddress = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        PasswordResetHash = c.String(),
                        PasswordResetTime = c.DateTime(),
                        Disabled = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AdminUserSessions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        IP = c.String(),
                        IsExpired = c.Boolean(nullable: false),
                        CookieGuid = c.Guid(nullable: false),
                        Created = c.DateTime(nullable: false),
                        AdminUserID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AdminUsers", t => t.AdminUserID)
                .Index(t => t.AdminUserID);
            
            CreateTable(
                "dbo.AdminUserAttempts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        IPAddress = c.String(),
                        When = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Photos",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FileGuid = c.String(),
                        Hash = c.String(),
                        OriginalFilename = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.AdminUserAdminRoles",
                c => new
                    {
                        AdminUser_ID = c.Int(nullable: false),
                        AdminRole_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.AdminUser_ID, t.AdminRole_ID })
                .ForeignKey("dbo.AdminUsers", t => t.AdminUser_ID, cascadeDelete: true)
                .ForeignKey("dbo.AdminRoles", t => t.AdminRole_ID, cascadeDelete: true)
                .Index(t => t.AdminUser_ID)
                .Index(t => t.AdminRole_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AdminUserSessions", "AdminUserID", "dbo.AdminUsers");
            DropForeignKey("dbo.AdminUserAdminRoles", "AdminRole_ID", "dbo.AdminRoles");
            DropForeignKey("dbo.AdminUserAdminRoles", "AdminUser_ID", "dbo.AdminUsers");
            DropIndex("dbo.AdminUserAdminRoles", new[] { "AdminRole_ID" });
            DropIndex("dbo.AdminUserAdminRoles", new[] { "AdminUser_ID" });
            DropIndex("dbo.AdminUserSessions", new[] { "AdminUserID" });
            DropTable("dbo.AdminUserAdminRoles");
            DropTable("dbo.Photos");
            DropTable("dbo.AdminUserAttempts");
            DropTable("dbo.AdminUserSessions");
            DropTable("dbo.AdminUsers");
            DropTable("dbo.AdminRoles");
        }
    }
}
