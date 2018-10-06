namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResumeRelatedupdates : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Certification",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        CertificationName = c.String(nullable: false),
                        CertificationAuthority = c.String(nullable: false),
                        LevelCertification = c.String(nullable: false),
                        FromYear = c.DateTime(nullable: false),
                        Level = c.Int(nullable: false),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.Education",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        InstituteUniversity = c.String(nullable: false),
                        TitleOfDiploma = c.String(nullable: false),
                        Degree = c.String(nullable: false),
                        FromYear = c.DateTime(nullable: false),
                        ToYear = c.DateTime(nullable: false),
                        City = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        LanguageName = c.String(nullable: false),
                        Proficiency = c.String(nullable: false),
                        LangProficiency = c.Int(nullable: false),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.Skill",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        SkillName = c.String(nullable: false),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.ServiceProvider_ID);
            
            CreateTable(
                "dbo.workExperience",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Company = c.String(nullable: false),
                        Title = c.String(nullable: false),
                        Country = c.String(nullable: false),
                        FromYear = c.DateTime(nullable: false),
                        ToYear = c.DateTime(nullable: false),
                        Description = c.String(nullable: false),
                        ServiceProvider_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProvider_ID)
                .Index(t => t.ServiceProvider_ID);
            
            AddColumn("dbo.ServiceProvider", "TwitterProfil", c => c.String(nullable: false));
            AddColumn("dbo.ServiceProvider", "Profil", c => c.Binary());
            AddColumn("dbo.ServiceProvider", "EducationalLevel", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.workExperience", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Skill", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Language", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Education", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Certification", "ServiceProvider_ID", "dbo.ServiceProvider");
            DropIndex("dbo.workExperience", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Skill", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Language", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Education", new[] { "ServiceProvider_ID" });
            DropIndex("dbo.Certification", new[] { "ServiceProvider_ID" });
            DropColumn("dbo.ServiceProvider", "EducationalLevel");
            DropColumn("dbo.ServiceProvider", "Profil");
            DropColumn("dbo.ServiceProvider", "TwitterProfil");
            DropTable("dbo.workExperience");
            DropTable("dbo.Skill");
            DropTable("dbo.Language");
            DropTable("dbo.Education");
            DropTable("dbo.Certification");
        }
    }
}
