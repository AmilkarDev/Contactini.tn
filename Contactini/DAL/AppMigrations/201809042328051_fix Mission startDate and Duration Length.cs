namespace Contactini.DAL.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixMissionstartDateandDurationLength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Mission", "StartDate", c => c.String(maxLength: 15));
            AlterColumn("dbo.Mission", "Duration", c => c.String(maxLength: 15));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Mission", "Duration", c => c.String());
            AlterColumn("dbo.Mission", "StartDate", c => c.String());
        }
    }
}
