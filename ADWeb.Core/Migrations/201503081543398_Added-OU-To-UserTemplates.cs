namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOUToUserTemplates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserTemplate", "DomainOUID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserTemplate", "DomainOUID");
            AddForeignKey("dbo.UserTemplate", "DomainOUID", "dbo.DomainOU", "DomainOUID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTemplate", "DomainOUID", "dbo.DomainOU");
            DropIndex("dbo.UserTemplate", new[] { "DomainOUID" });
            DropColumn("dbo.UserTemplate", "DomainOUID");
        }
    }
}
