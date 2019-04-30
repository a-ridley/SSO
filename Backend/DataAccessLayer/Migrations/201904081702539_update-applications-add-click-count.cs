namespace DataAccessLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateapplicationsaddclickcount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Applications", "ClickCount", c => c.Long(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Applications", "ClickCount");
        }
    }
}
