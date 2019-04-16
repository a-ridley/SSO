namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateapplicationsaddHealthCheckUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applications", "HealthCheckUrl", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applications", "HealthCheckUrl");
        }
    }
}
