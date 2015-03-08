namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedTemplateGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTemplate", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.DomainGroup", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.DomainGroup", "DN", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DomainGroup", "DN", c => c.String());
            AlterColumn("dbo.DomainGroup", "Name", c => c.String());
            DropColumn("dbo.UserTemplate", "Name");
        }
    }
}
