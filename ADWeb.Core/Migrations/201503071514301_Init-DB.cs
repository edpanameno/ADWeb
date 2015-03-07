namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomainUser",
                c => new
                    {
                        Username = c.String(nullable: false, maxLength: 128),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Username);
            
            CreateTable(
                "dbo.UserUpdateHistory",
                c => new
                    {
                        UserUpdateHistoryID = c.Int(nullable: false, identity: true),
                        UpdatedBy = c.String(),
                        DateUpdated = c.DateTime(nullable: false),
                        UpdateType = c.Int(nullable: false),
                        Notes = c.String(),
                        Username = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.UserUpdateHistoryID)
                .ForeignKey("dbo.DomainUser", t => t.Username)
                .Index(t => t.Username);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserUpdateHistory", "Username", "dbo.DomainUser");
            DropIndex("dbo.UserUpdateHistory", new[] { "Username" });
            DropTable("dbo.UserUpdateHistory");
            DropTable("dbo.DomainUser");
        }
    }
}
