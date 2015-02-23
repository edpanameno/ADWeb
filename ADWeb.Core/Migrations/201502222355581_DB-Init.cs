namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DBInit : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.DomainUsers", newName: "DomainUser");
            RenameTable(name: "dbo.UserUpdateHistories", newName: "UserUpdateHistory");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.UserUpdateHistory", newName: "UserUpdateHistories");
            RenameTable(name: "dbo.DomainUser", newName: "DomainUsers");
        }
    }
}
