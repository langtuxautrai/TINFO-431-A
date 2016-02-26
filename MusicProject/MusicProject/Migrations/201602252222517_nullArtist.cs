namespace MusicProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nullArtist : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Albums", new[] { "ArtistID" });
            AlterColumn("dbo.Albums", "ArtistID", c => c.Int());
            CreateIndex("dbo.Albums", "ArtistID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Albums", new[] { "ArtistID" });
            AlterColumn("dbo.Albums", "ArtistID", c => c.Int(nullable: false));
            CreateIndex("dbo.Albums", "ArtistID");
        }
    }
}
