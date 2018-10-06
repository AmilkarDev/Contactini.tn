namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MessagemodelRequiredupdates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Message", "Title", c => c.String(nullable: false));
            AlterColumn("dbo.Message", "Content", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Message", "Content", c => c.String());
            AlterColumn("dbo.Message", "Title", c => c.String());
        }
    }
}
