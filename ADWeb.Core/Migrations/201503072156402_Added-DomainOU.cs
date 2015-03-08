namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedDomainOU : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomainOU",
                c => new
                    {
                        DomainOUID = c.Int(nullable: false, identity: true),
                        DistinguishedName = c.String(),
                        Name = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.DomainOUID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.DomainOU");
        }
    }
}
