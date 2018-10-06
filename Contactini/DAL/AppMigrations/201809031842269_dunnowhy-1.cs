namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dunnowhy1 : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.EditUserViewModel");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.EditUserViewModel",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        FullName = c.String(nullable: false),
                        UserName = c.String(nullable: false),
                        Email = c.String(nullable: false),
                        PhoneNumber = c.String(),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        PostalCode = c.String(),
                        PhotoLink = c.String(),
                        isAdmin = c.Boolean(nullable: false),
                        isServiceProvider = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}
