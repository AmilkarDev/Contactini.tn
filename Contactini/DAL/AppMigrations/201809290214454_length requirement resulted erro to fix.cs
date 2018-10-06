namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lengthrequirementresultederrotofix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ServiceProvider", "Titre", c => c.String(maxLength: 55));
            AlterColumn("dbo.Domain", "Name", c => c.String(nullable: false, maxLength: 35));
            AlterColumn("dbo.Realisation", "TakenTime", c => c.String(maxLength: 35));
            AlterColumn("dbo.Sector", "Name", c => c.String(nullable: false, maxLength: 45));
            AlterColumn("dbo.Message", "Title", c => c.String(nullable: false, maxLength: 45));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Message", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Sector", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Realisation", "TakenTime", c => c.String());
            AlterColumn("dbo.Domain", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.ServiceProvider", "Titre", c => c.String());
        }
    }
}
