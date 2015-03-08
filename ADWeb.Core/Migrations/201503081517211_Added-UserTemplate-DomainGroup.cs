namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserTemplateDomainGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DomainGroup",
                c => new
                    {
                        DomainGroupID = c.Int(nullable: false, identity: true),
                        Enabled = c.Boolean(nullable: false),
                        Name = c.String(),
                        DN = c.String(),
                        UserTemplateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DomainGroupID)
                .ForeignKey("dbo.UserTemplate", t => t.UserTemplateID, cascadeDelete: true)
                .Index(t => t.UserTemplateID);
            
            CreateTable(
                "dbo.UserTemplate",
                c => new
                    {
                        UserTemplateID = c.Int(nullable: false, identity: true),
                        Enabled = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.String(),
                        ChangePasswordAtNextLogon = c.Boolean(),
                        UserCannotChangePassword = c.Boolean(),
                        PasswordNeverExpires = c.Boolean(),
                        AccountExpires = c.Boolean(),
                        Notes = c.String(),
                    })
                .PrimaryKey(t => t.UserTemplateID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DomainGroup", "UserTemplateID", "dbo.UserTemplate");
            DropIndex("dbo.DomainGroup", new[] { "UserTemplateID" });
            DropTable("dbo.UserTemplate");
            DropTable("dbo.DomainGroup");
        }
    }
}
