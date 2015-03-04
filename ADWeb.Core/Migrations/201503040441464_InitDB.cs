namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserUpdateHistory", "Notes", c => c.String());
            AddColumn("dbo.UserUpdateHistory", "DomainUserID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserUpdateHistory", "DomainUserID");
            AddForeignKey("dbo.UserUpdateHistory", "DomainUserID", "dbo.DomainUser", "DomainUserID", cascadeDelete: true);
            DropColumn("dbo.DomainUser", "Enabled");
            DropColumn("dbo.DomainUser", "FirstName");
            DropColumn("dbo.DomainUser", "MiddleName");
            DropColumn("dbo.DomainUser", "LastName");
            DropColumn("dbo.UserUpdateHistory", "UserName");
            DropColumn("dbo.UserUpdateHistory", "UpdateHistory");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserUpdateHistory", "UpdateHistory", c => c.String());
            AddColumn("dbo.UserUpdateHistory", "UserName", c => c.String());
            AddColumn("dbo.DomainUser", "LastName", c => c.String());
            AddColumn("dbo.DomainUser", "MiddleName", c => c.String());
            AddColumn("dbo.DomainUser", "FirstName", c => c.String());
            AddColumn("dbo.DomainUser", "Enabled", c => c.Boolean(nullable: false));
            DropForeignKey("dbo.UserUpdateHistory", "DomainUserID", "dbo.DomainUser");
            DropIndex("dbo.UserUpdateHistory", new[] { "DomainUserID" });
            DropColumn("dbo.UserUpdateHistory", "DomainUserID");
            DropColumn("dbo.UserUpdateHistory", "Notes");
        }
    }
}
