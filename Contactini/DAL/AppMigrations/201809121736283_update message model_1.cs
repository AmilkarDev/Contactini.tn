namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemessagemodel_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Message", "senderName", c => c.String());
            AddColumn("dbo.Message", "senderPhone", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Message", "senderPhone");
            DropColumn("dbo.Message", "senderName");
        }
    }
}
