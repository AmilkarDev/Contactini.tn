namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecertificatemodel : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Certification", "Level");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Certification", "Level", c => c.Int(nullable: false));
        }
    }
}
