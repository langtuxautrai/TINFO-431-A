namespace MusicProject.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NullFK : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Albums",
                c => new
                    {
                        AlbumID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        ArtistID = c.Int(nullable: false),
                        Genres = c.String(),
                        Producers = c.String(),
                        CompanyID = c.Int(),
                        Release = c.DateTime(),
                    })
                .PrimaryKey(t => t.AlbumID)
                .ForeignKey("dbo.Artists", t => t.ArtistID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .Index(t => t.ArtistID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Artists",
                c => new
                    {
                        ArtistID = c.Int(nullable: false, identity: true),
                        Fname = c.String(),
                        Lname = c.String(),
                        Genres = c.String(),
                        Debut_in = c.DateTime(),
                        History = c.String(),
                        CompanyID = c.Int(),
                        Rewards = c.String(),
                    })
                .PrimaryKey(t => t.ArtistID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        CompanyID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Address = c.String(),
                        phone = c.String(),
                        Website = c.String(),
                        Found = c.DateTime(),
                    })
                .PrimaryKey(t => t.CompanyID);
            
            CreateTable(
                "dbo.Composers",
                c => new
                    {
                        ComposerID = c.Int(nullable: false, identity: true),
                        Fname = c.String(),
                        Lname = c.String(),
                        Genres = c.String(),
                        CompanyID = c.Int(),
                        Rewards = c.String(),
                    })
                .PrimaryKey(t => t.ComposerID)
                .ForeignKey("dbo.Companies", t => t.CompanyID)
                .Index(t => t.CompanyID);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        SongID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Genres = c.String(),
                        ArtistID = c.Int(nullable: false),
                        ComposerID = c.Int(),
                        AlbumID = c.Int(),
                        Release = c.DateTime(),
                        Peak_position = c.String(),
                        Lyric = c.String(),
                    })
                .PrimaryKey(t => t.SongID)
                .ForeignKey("dbo.Albums", t => t.AlbumID)
                .ForeignKey("dbo.Artists", t => t.ArtistID)
                .ForeignKey("dbo.Composers", t => t.ComposerID)
                .Index(t => t.ArtistID)
                .Index(t => t.ComposerID)
                .Index(t => t.AlbumID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Albums", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Albums", "ArtistID", "dbo.Artists");
            DropForeignKey("dbo.Artists", "CompanyID", "dbo.Companies");
            DropForeignKey("dbo.Songs", "ComposerID", "dbo.Composers");
            DropForeignKey("dbo.Songs", "ArtistID", "dbo.Artists");
            DropForeignKey("dbo.Songs", "AlbumID", "dbo.Albums");
            DropForeignKey("dbo.Composers", "CompanyID", "dbo.Companies");
            DropIndex("dbo.Songs", new[] { "AlbumID" });
            DropIndex("dbo.Songs", new[] { "ComposerID" });
            DropIndex("dbo.Songs", new[] { "ArtistID" });
            DropIndex("dbo.Composers", new[] { "CompanyID" });
            DropIndex("dbo.Artists", new[] { "CompanyID" });
            DropIndex("dbo.Albums", new[] { "CompanyID" });
            DropIndex("dbo.Albums", new[] { "ArtistID" });
            DropTable("dbo.Songs");
            DropTable("dbo.Composers");
            DropTable("dbo.Companies");
            DropTable("dbo.Artists");
            DropTable("dbo.Albums");
        }
    }
}
