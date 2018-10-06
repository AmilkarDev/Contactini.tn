namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Addrequiredtocandidaturetextandtitleproperties : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Candidature", "Texte", c => c.String(nullable: false));
            AlterColumn("dbo.Candidature", "Title", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Candidature", "Title", c => c.String());
            AlterColumn("dbo.Candidature", "Texte", c => c.String());
        }
    }
}
