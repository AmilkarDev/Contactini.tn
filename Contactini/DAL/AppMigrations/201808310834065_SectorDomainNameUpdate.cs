namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SectorDomainNameUpdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Domain", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Sector", "Name", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Sector", "Name", c => c.String());
            AlterColumn("dbo.Domain", "Name", c => c.String());
        }
    }
}
