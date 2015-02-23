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
                        CreatedBy = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        UserName = c.String(),
                        Enabled = c.Boolean(nullable: false),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.DomainUserID);
            
            CreateTable(
                "dbo.UserUpdateHistory",
                c => new
                    {
                        UserUpdateHistoryID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UpdatedBy = c.String(),
                        DateUpdated = c.DateTime(nullable: false),
                        UpdateType = c.Int(nullable: false),
                        UpdateHistory = c.String(),
                    })
                .PrimaryKey(t => t.UserUpdateHistoryID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.UserUpdateHistory");
            DropTable("dbo.DomainUser");
        }
    }
}
