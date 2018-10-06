namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDomainPhotoLink : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Domain", "Photo_ID", "dbo.Photo");
            DropIndex("dbo.Domain", new[] { "Photo_ID" });
            AddColumn("dbo.Domain", "PhotoLink", c => c.String());
            DropColumn("dbo.Domain", "Photo_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Domain", "Photo_ID", c => c.Int());
            DropColumn("dbo.Domain", "PhotoLink");
            CreateIndex("dbo.Domain", "Photo_ID");
            AddForeignKey("dbo.Domain", "Photo_ID", "dbo.Photo", "ID");
        }
    }
}
