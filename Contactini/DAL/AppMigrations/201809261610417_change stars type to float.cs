namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changestarstypetofloat : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Realisation", "Stars", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Realisation", "Stars", c => c.Int(nullable: false));
        }
    }
}
