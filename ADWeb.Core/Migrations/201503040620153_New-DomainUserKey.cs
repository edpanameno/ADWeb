namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewDomainUserKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserUpdateHistory", "DomainUserID", "dbo.DomainUser");
            DropIndex("dbo.UserUpdateHistory", new[] { "DomainUserID" });
            DropPrimaryKey("dbo.DomainUser");
            AddColumn("dbo.UserUpdateHistory", "DomainUser_Username", c => c.String(maxLength: 128));
            AlterColumn("dbo.DomainUser", "DomainUserID", c => c.Int(nullable: false));
            AlterColumn("dbo.DomainUser", "Username", c => c.String(nullable: false, maxLength: 128));
            AddPrimaryKey("dbo.DomainUser", "Username");
            CreateIndex("dbo.UserUpdateHistory", "DomainUser_Username");
            AddForeignKey("dbo.UserUpdateHistory", "DomainUser_Username", "dbo.DomainUser", "Username");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserUpdateHistory", "DomainUser_Username", "dbo.DomainUser");
            DropIndex("dbo.UserUpdateHistory", new[] { "DomainUser_Username" });
            DropPrimaryKey("dbo.DomainUser");
            AlterColumn("dbo.DomainUser", "Username", c => c.String());
            AlterColumn("dbo.DomainUser", "DomainUserID", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.UserUpdateHistory", "DomainUser_Username");
            AddPrimaryKey("dbo.DomainUser", "DomainUserID");
            CreateIndex("dbo.UserUpdateHistory", "DomainUserID");
            AddForeignKey("dbo.UserUpdateHistory", "DomainUserID", "dbo.DomainUser", "DomainUserID", cascadeDelete: true);
        }
    }
}
