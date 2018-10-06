namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemessagemodel0 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Message", "PhoneNumber", c => c.String());
            AddColumn("dbo.Message", "senderEmail", c => c.String());
            AddColumn("dbo.Message", "ReceiverEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Message", "ReceiverEmail");
            DropColumn("dbo.Message", "senderEmail");
            DropColumn("dbo.Message", "PhoneNumber");
        }
    }
}
