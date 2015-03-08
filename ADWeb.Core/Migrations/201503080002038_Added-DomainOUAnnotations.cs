namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDomainOUAnnotations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DomainOU", "DistinguishedName", c => c.String(nullable: false));
            AlterColumn("dbo.DomainOU", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.DomainOU", "Name", c => c.String());
            AlterColumn("dbo.DomainOU", "DistinguishedName", c => c.String());
        }
    }
}
