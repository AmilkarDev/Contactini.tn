namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addfavPrestalisttoclientmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceProvider", "Client_ID", c => c.Int());
            CreateIndex("dbo.ServiceProvider", "Client_ID");
            AddForeignKey("dbo.ServiceProvider", "Client_ID", "dbo.Client", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceProvider", "Client_ID", "dbo.Client");
            DropIndex("dbo.ServiceProvider", new[] { "Client_ID" });
            DropColumn("dbo.ServiceProvider", "Client_ID");
        }
    }
}
