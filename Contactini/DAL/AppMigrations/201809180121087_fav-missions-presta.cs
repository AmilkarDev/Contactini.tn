namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class favmissionspresta : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mission", "ServiceProvider_ID", "dbo.ServiceProvider");
            AddColumn("dbo.Mission", "ServiceProvider_ID1", c => c.Int());
            AddColumn("dbo.Mission", "ServiceProvider_ID2", c => c.Int());
            CreateIndex("dbo.Mission", "ServiceProvider_ID1");
            CreateIndex("dbo.Mission", "ServiceProvider_ID2");
            AddForeignKey("dbo.Mission", "ServiceProvider_ID", "dbo.ServiceProvider", "ID");
            AddForeignKey("dbo.Mission", "ServiceProvider_ID2", "dbo.ServiceProvider", "ID");
            AddForeignKey("dbo.Mission", "ServiceProvider_ID1", "dbo.ServiceProvider", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Mission", "ServiceProvider_ID1", "dbo.ServiceProvider");
            DropForeignKey("dbo.Mission", "ServiceProvider_ID2", "dbo.ServiceProvider");
            DropForeignKey("dbo.Mission", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropIndex("dbo.Mission", new[] { "ServiceProvider_ID2" });
            DropIndex("dbo.Mission", new[] { "ServiceProvider_ID1" });
            DropColumn("dbo.Mission", "ServiceProvider_ID2");
            DropColumn("dbo.Mission", "ServiceProvider_ID1");
            AddForeignKey("dbo.Mission", "ServiceProvider_ID", "dbo.ServiceProvider", "ID");
        }
    }
}
