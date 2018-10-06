namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResumeRelatedfbtwitterupdates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceProvider", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.ServiceProvider", "LinkedInProfil", c => c.String());
            AddColumn("dbo.ServiceProvider", "FacebookProfil", c => c.String());
            AlterColumn("dbo.ServiceProvider", "TwitterProfil", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ServiceProvider", "TwitterProfil", c => c.String(nullable: false));
            DropColumn("dbo.ServiceProvider", "FacebookProfil");
            DropColumn("dbo.ServiceProvider", "LinkedInProfil");
            DropColumn("dbo.ServiceProvider", "DateOfBirth");
        }
    }
}
