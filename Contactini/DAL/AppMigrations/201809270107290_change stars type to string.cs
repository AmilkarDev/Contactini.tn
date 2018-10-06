namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestarstypetostring : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Realisation", "Stars", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Realisation", "Stars", c => c.Single(nullable: false));
        }
    }
}
