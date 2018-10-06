namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MissionUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Mission", "Sector_ID", c => c.Int());
            CreateIndex("dbo.Mission", "Sector_ID");
            AddForeignKey("dbo.Mission", "Sector_ID", "dbo.Sector", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mission", "Sector_ID", "dbo.Sector");
            DropIndex("dbo.Mission", new[] { "Sector_ID" });
            DropColumn("dbo.Mission", "Sector_ID");
        }
    }
}
