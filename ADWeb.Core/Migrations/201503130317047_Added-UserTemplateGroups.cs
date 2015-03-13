namespace ADWeb.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedUserTemplateGroups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserTemplateGroup",
                c => new
                    {
                        UserTemplateGroupID = c.Int(nullable: false, identity: true),
                        Enabled = c.Boolean(nullable: false),
                        Name = c.String(),
                        DistinguishedName = c.String(),
                        UserTemplateID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserTemplateGroupID)
                .Index(t => t.UserTemplateID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.UserTemplateGroup", new[] { "UserTemplateID" });
            DropTable("dbo.UserTemplateGroup");
        }
    }
}
