namespace Contactini.DAL.IdentityMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ProfilePicturUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "PhotoLink", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "PhotoLink");
        }
    }
}
