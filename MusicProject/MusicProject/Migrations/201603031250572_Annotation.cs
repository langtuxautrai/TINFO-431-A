namespace MusicProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Annotation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Albums", "Title", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Artists", "Fname", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Artists", "Lname", c => c.String(maxLength: 50));
            AlterColumn("dbo.Companies", "Name", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Composers", "Fname", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Composers", "Lname", c => c.String(maxLength: 50));
            AlterColumn("dbo.Songs", "Title", c => c.String(nullable: false, maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Songs", "Title", c => c.String());
            AlterColumn("dbo.Composers", "Lname", c => c.String());
            AlterColumn("dbo.Composers", "Fname", c => c.String());
            AlterColumn("dbo.Companies", "Name", c => c.String());
            AlterColumn("dbo.Artists", "Lname", c => c.String());
            AlterColumn("dbo.Artists", "Fname", c => c.String());
            AlterColumn("dbo.Albums", "Title", c => c.String());
        }
    }
}
