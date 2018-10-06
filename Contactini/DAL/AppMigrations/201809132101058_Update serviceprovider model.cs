namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Updateserviceprovidermodel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ServiceProviderDomain", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.ServiceProviderDomain", "Domain_domainId", "dbo.Domain");
            DropIndex("dbo.ServiceProviderDomain", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.ServiceProviderDomain", new[] { "Domain_domainId" });
            AddColumn("dbo.ServiceProvider", "Titre", c => c.String());
            AddColumn("dbo.ServiceProvider", "Domain_domainId", c => c.Int());
            AddColumn("dbo.ServiceProvider", "sector_ID", c => c.Int());
            CreateIndex("dbo.ServiceProvider", "Domain_domainId");
            CreateIndex("dbo.ServiceProvider", "sector_ID");
            AddForeignKey("dbo.ServiceProvider", "Domain_domainId", "dbo.Domain", "domainId");
            AddForeignKey("dbo.ServiceProvider", "sector_ID", "dbo.Sector", "ID");
            DropTable("dbo.ServiceProviderDomain");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ServiceProviderDomain",
                c => new
                    {
                        ServiceProvider_ID = c.Int(nullable: false),
                        Domain_domainId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ServiceProvider_ID, t.Domain_domainId });
            
            DropForeignKey("dbo.ServiceProvider", "sector_ID", "dbo.Sector");
            DropForeignKey("dbo.ServiceProvider", "Domain_domainId", "dbo.Domain");
            DropIndex("dbo.ServiceProvider", new[] { "sector_ID" });
            DropIndex("dbo.ServiceProvider", new[] { "Domain_domainId" });
            DropColumn("dbo.ServiceProvider", "sector_ID");
            DropColumn("dbo.ServiceProvider", "Domain_domainId");
            DropColumn("dbo.ServiceProvider", "Titre");
            CreateIndex("dbo.ServiceProviderDomain", "Domain_domainId");
            CreateIndex("dbo.ServiceProviderDomain", "ServiceProvider_ID");
            AddForeignKey("dbo.ServiceProviderDomain", "Domain_domainId", "dbo.Domain", "domainId", cascadeDelete: true);
            AddForeignKey("dbo.ServiceProviderDomain", "ServiceProvider_ID", "dbo.ServiceProvider", "ID", cascadeDelete: true);
        }
    }
}
