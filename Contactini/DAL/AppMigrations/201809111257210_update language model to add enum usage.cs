namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatelanguagemodeltoaddenumusage : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Language", "Proficiency");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Language", "Proficiency", c => c.String(nullable: false));
        }
    }
}
