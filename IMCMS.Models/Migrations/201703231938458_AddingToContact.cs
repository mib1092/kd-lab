namespace IMCMS.Models.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingToContact : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Contacts", "Options", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Contacts", "Options");
        }
    }
}
