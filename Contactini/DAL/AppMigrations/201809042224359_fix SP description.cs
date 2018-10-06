namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixSPdescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceProvider", "Description", c => c.String());
            DropColumn("dbo.ServiceProvider", "Descripption");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ServiceProvider", "Descripption", c => c.String());
            DropColumn("dbo.ServiceProvider", "Description");
        }
    }
}
