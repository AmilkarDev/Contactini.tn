namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecertificatemodel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Certification", "Level", c => c.Int(nullable: false));
            DropColumn("dbo.Certification", "LevelCertification");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Certification", "LevelCertification", c => c.String(nullable: false));
            DropColumn("dbo.Certification", "Level");
        }
    }
}
