namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingToJob : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Jobs", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Jobs", "IsDefault");
        }
    }
}
