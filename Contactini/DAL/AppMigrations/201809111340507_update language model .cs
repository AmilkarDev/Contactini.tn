namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatelanguagemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Language", "Proficiency", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Language", "Proficiency");
        }
    }
}
