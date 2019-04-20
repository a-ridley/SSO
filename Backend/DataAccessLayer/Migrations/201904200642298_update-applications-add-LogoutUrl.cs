namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateapplicationsaddLogoutUrl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applications", "LogoutUrl", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applications", "LogoutUrl");
        }
    }
}
