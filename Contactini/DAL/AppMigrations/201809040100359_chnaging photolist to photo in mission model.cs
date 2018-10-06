namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chnagingphotolisttophotoinmissionmodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Photo", "Mission_ID", "dbo.Mission");
            DropIndex("dbo.Photo", new[] { "Mission_ID" });
            AddColumn("dbo.Mission", "PhotoLink", c => c.String());
            DropColumn("dbo.Photo", "Mission_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Photo", "Mission_ID", c => c.Int());
            DropColumn("dbo.Mission", "PhotoLink");
            CreateIndex("dbo.Photo", "Mission_ID");
            AddForeignKey("dbo.Photo", "Mission_ID", "dbo.Mission", "ID");
        }
    }
}
