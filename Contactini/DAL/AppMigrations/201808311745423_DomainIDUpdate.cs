namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DomainIDUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Mission", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.Realisation", "Domain_ID", "dbo.Domain");
            DropForeignKey("dbo.ServiceProviderDomain", "Domain_ID", "dbo.Domain");
            DropPrimaryKey("dbo.Domain");
            DropColumn("dbo.Domain", "ID");
            RenameColumn(table: "dbo.Mission", name: "Domain_ID", newName: "Domain_domainId");
            RenameColumn(table: "dbo.Realisation", name: "Domain_ID", newName: "Domain_domainId");
            RenameColumn(table: "dbo.ServiceProviderDomain", name: "Domain_ID", newName: "Domain_domainId");
            RenameIndex(table: "dbo.Mission", name: "IX_Domain_ID", newName: "IX_Domain_domainId");
            RenameIndex(table: "dbo.Realisation", name: "IX_Domain_ID", newName: "IX_Domain_domainId");
            RenameIndex(table: "dbo.ServiceProviderDomain", name: "IX_Domain_ID", newName: "IX_Domain_domainId");
            
            AddColumn("dbo.Domain", "domainId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Domain", "domainId");
            AddForeignKey("dbo.Mission", "Domain_domainId", "dbo.Domain", "domainId");
            AddForeignKey("dbo.Realisation", "Domain_domainId", "dbo.Domain", "domainId");
            AddForeignKey("dbo.ServiceProviderDomain", "Domain_domainId", "dbo.Domain", "domainId", cascadeDelete: true);
           
        }
        
        public override void Down()
        {
            AddColumn("dbo.Domain", "ID", c => c.Int(nullable: false, identity: true));
            DropForeignKey("dbo.ServiceProviderDomain", "Domain_domainId", "dbo.Domain");
            DropForeignKey("dbo.Realisation", "Domain_domainId", "dbo.Domain");
            DropForeignKey("dbo.Mission", "Domain_domainId", "dbo.Domain");
            DropPrimaryKey("dbo.Domain");
            DropColumn("dbo.Domain", "domainId");
            AddPrimaryKey("dbo.Domain", "ID");
            RenameIndex(table: "dbo.ServiceProviderDomain", name: "IX_Domain_domainId", newName: "IX_Domain_ID");
            RenameIndex(table: "dbo.Realisation", name: "IX_Domain_domainId", newName: "IX_Domain_ID");
            RenameIndex(table: "dbo.Mission", name: "IX_Domain_domainId", newName: "IX_Domain_ID");
            RenameColumn(table: "dbo.ServiceProviderDomain", name: "Domain_domainId", newName: "Domain_ID");
            RenameColumn(table: "dbo.Realisation", name: "Domain_domainId", newName: "Domain_ID");
            RenameColumn(table: "dbo.Mission", name: "Domain_domainId", newName: "Domain_ID");
            AddForeignKey("dbo.ServiceProviderDomain", "Domain_ID", "dbo.Domain", "ID", cascadeDelete: true);
            AddForeignKey("dbo.Realisation", "Domain_ID", "dbo.Domain", "ID");
            AddForeignKey("dbo.Mission", "Domain_ID", "dbo.Domain", "ID");
        }
    }
}
