namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewUserUpdateHistoryKey : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.UserUpdateHistory", name: "DomainUser_Username", newName: "Username");
            RenameIndex(table: "dbo.UserUpdateHistory", name: "IX_DomainUser_Username", newName: "IX_Username");
            DropColumn("dbo.UserUpdateHistory", "DomainUserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserUpdateHistory", "DomainUserID", c => c.Int(nullable: false));
            RenameIndex(table: "dbo.UserUpdateHistory", name: "IX_Username", newName: "IX_DomainUser_Username");
            RenameColumn(table: "dbo.UserUpdateHistory", name: "Username", newName: "DomainUser_Username");
        }
    }
}
