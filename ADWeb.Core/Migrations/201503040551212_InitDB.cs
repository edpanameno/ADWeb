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
                        DomainUserID = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.DomainUserID);
            
            CreateTable(
                "dbo.UserUpdateHistory",
                c => new
                    {
                        UserUpdateHistoryID = c.Int(nullable: false, identity: true),
                        UpdatedBy = c.String(),
                        DateUpdated = c.DateTime(nullable: false),
                        UpdateType = c.Int(nullable: false),
                        Notes = c.String(),
                        DomainUserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserUpdateHistoryID)
                .ForeignKey("dbo.DomainUser", t => t.DomainUserID, cascadeDelete: true)
                .Index(t => t.DomainUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserUpdateHistory", "DomainUserID", "dbo.DomainUser");
            DropIndex("dbo.UserUpdateHistory", new[] { "DomainUserID" });
            DropTable("dbo.UserUpdateHistory");
            DropTable("dbo.DomainUser");
        }
    }
}
