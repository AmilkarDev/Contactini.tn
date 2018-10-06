namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        StreetAddress = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        PostalCode = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Candidature",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        State = c.Int(nullable: false),
                        AppDate = c.DateTime(nullable: false),
                        Texte = c.String(),
                        Title = c.String(),
                        Mission_ID = c.Int(),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Mission", t => t.Mission_ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.Mission_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.Mission",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        Title = c.String(),
                        PublishDate = c.DateTime(nullable: false),
                        StartDate = c.String(),
                        Duration = c.String(),
                        State = c.Int(nullable: false),
                        Address_ID = c.Int(),
                        Client_ID = c.Int(),
                        Domain_ID = c.Int(),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Address", t => t.Address_ID)
                .ForeignKey("dbo.Client", t => t.Client_ID)
                .ForeignKey("dbo.Domain", t => t.Domain_ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.Address_ID)
                .Index(t => t.Client_ID)
                .Index(t => t.Domain_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.Client",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        UserName = c.String(),
                        Email = c.String(),
                        PhoneNum = c.String(),
                        Address_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Address", t => t.Address_ID)
                .Index(t => t.Address_ID);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Content = c.String(),
                        Client_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Client", t => t.Client_ID)
                .Index(t => t.Client_ID);
            
            CreateTable(
                "dbo.Realisation",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Opinion = c.String(),
                        Stars = c.Int(nullable: false),
                        TakenTime = c.String(),
                        Client_ID = c.Int(),
                        Domain_ID = c.Int(),
                        ServiceProvider_ID = c.Int(),
                        Mission_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Client", t => t.Client_ID)
                .ForeignKey("dbo.Domain", t => t.Domain_ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .ForeignKey("dbo.Mission", t => t.Mission_ID)
                .Index(t => t.Client_ID)
                .Index(t => t.Domain_ID)
                .Index(t => t.ServiceProvider_ID)
                .Index(t => t.Mission_ID);
            
            CreateTable(
                "dbo.Domain",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        MissionCount = c.Int(nullable: false),
                        Photo_ID = c.Int(),
                        Sector_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Photo", t => t.Photo_ID)
                .ForeignKey("dbo.Sector", t => t.Sector_ID)
                .Index(t => t.Photo_ID)
                .Index(t => t.Sector_ID);
            
            CreateTable(
                "dbo.Photo",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Link = c.String(),
                        Name = c.String(),
                        Description = c.String(),
                        ServiceProvider_ID = c.Int(),
                        Realisation_ID = c.Int(),
                        Mission_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .ForeignKey("dbo.Realisation", t => t.Realisation_ID)
                .ForeignKey("dbo.Mission", t => t.Mission_ID)
                .Index(t => t.ServiceProvider_ID)
                .Index(t => t.Realisation_ID)
                .Index(t => t.Mission_ID);
            
            CreateTable(
                "dbo.ServiceProvider",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Email = c.String(),
                        PhoneNum = c.String(),
                        UserName = c.String(),
                        Descripption = c.String(),
                        Diponibility = c.Boolean(nullable: false),
                        Stars = c.Int(nullable: false),
                        HasDrivingLicence = c.Boolean(nullable: false),
                        HasPassport = c.Boolean(nullable: false),
                        HasACar = c.Boolean(nullable: false),
                        Address_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Address", t => t.Address_ID)
                .Index(t => t.Address_ID);
            
            CreateTable(
                "dbo.Sector",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ServiceProviderDomain",
                c => new
                    {
                        ServiceProvider_ID = c.Int(nullable: false),
                        Domain_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceProvider_ID, t.Domain_ID })
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID, cascadeDelete: true)
                .ForeignKey("dbo.Domain", t => t.Domain_ID, cascadeDelete: true)
                .Index(t => t.ServiceProvider_ID)
                .Index(t => t.Domain_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Domain", "Sector_ID", "dbo.Sector");
            DropForeignKey("dbo.Photo", "Mission_ID", "dbo.Mission");
            DropForeignKey("dbo.Photo", "Realisation_ID", "dbo.Realisation");
            DropForeignKey("dbo.Realisation", "Mission_ID", "dbo.Mission");
            DropForeignKey("dbo.Realisation", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Photo", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Mission", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.ServiceProviderDomain", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.ServiceProviderDomain", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Candidature", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.ServiceProvider", "Address_ID", "dbo.Address");
            DropForeignKey("dbo.Realisation", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.Domain", "Photo_ID", "dbo.Photo");
            DropForeignKey("dbo.Mission", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.Realisation", "Client_ID", "dbo.Client");
            DropForeignKey("dbo.Mission", "Client_ID", "dbo.Client");
            DropForeignKey("dbo.Message", "Client_ID", "dbo.Client");
            DropForeignKey("dbo.Client", "Address_ID", "dbo.Address");
            DropForeignKey("dbo.Candidature", "Mission_ID", "dbo.Mission");
            DropForeignKey("dbo.Mission", "Address_ID", "dbo.Address");
            DropIndex("dbo.ServiceProviderDomain", new[] { "Domain_ID" });
            DropIndex("dbo.ServiceProviderDomain", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.ServiceProvider", new[] { "Address_ID" });
            DropIndex("dbo.Photo", new[] { "Mission_ID" });
            DropIndex("dbo.Photo", new[] { "Realisation_ID" });
            DropIndex("dbo.Photo", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Domain", new[] { "Sector_ID" });
            DropIndex("dbo.Domain", new[] { "Photo_ID" });
            DropIndex("dbo.Realisation", new[] { "Mission_ID" });
            DropIndex("dbo.Realisation", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Realisation", new[] { "Domain_ID" });
            DropIndex("dbo.Realisation", new[] { "Client_ID" });
            DropIndex("dbo.Message", new[] { "Client_ID" });
            DropIndex("dbo.Client", new[] { "Address_ID" });
            DropIndex("dbo.Mission", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Mission", new[] { "Domain_ID" });
            DropIndex("dbo.Mission", new[] { "Client_ID" });
            DropIndex("dbo.Mission", new[] { "Address_ID" });
            DropIndex("dbo.Candidature", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Candidature", new[] { "Mission_ID" });
            DropTable("dbo.ServiceProviderDomain");
            DropTable("dbo.Sector");
            DropTable("dbo.ServiceProvider");
            DropTable("dbo.Photo");
            DropTable("dbo.Domain");
            DropTable("dbo.Realisation");
            DropTable("dbo.Message");
            DropTable("dbo.Client");
            DropTable("dbo.Mission");
            DropTable("dbo.Candidature");
            DropTable("dbo.Address");
        }
    }
}
