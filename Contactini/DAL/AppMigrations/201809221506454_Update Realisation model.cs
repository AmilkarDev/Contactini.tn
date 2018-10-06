namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateRealisationmodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Realisation", "Description", c => c.String());
            AddColumn("dbo.Realisation", "Validation", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Realisation", "Validation");
            DropColumn("dbo.Realisation", "Description");
        }
    }
}
