namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createupdatedtablesapplicationsapiKeys : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        LaunchUrl = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        LogoUrl = c.String(),
                        Description = c.String(),
                        UserDeletionUrl = c.String(nullable: false),
                        SharedSecretKey = c.String(nullable: false),
                        UnderMaintenance = c.Boolean(nullable: false),
                        ClickCount = c.Long(nullable: false),
                        HealthCheckUrl = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ApiKeys",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Key = c.String(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        IsUsed = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applications", t => t.ApplicationId, cascadeDelete: true)
                .Index(t => t.ApplicationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ApiKeys", "ApplicationId", "dbo.Applications");
            DropIndex("dbo.ApiKeys", new[] { "ApplicationId" });
            DropTable("dbo.ApiKeys");
            DropTable("dbo.Applications");
        }
    }
}
