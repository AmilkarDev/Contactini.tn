namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateserviceprovidermodeladdingphotoLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceProvider", "photoLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ServiceProvider", "photoLink");
        }
    }
}
