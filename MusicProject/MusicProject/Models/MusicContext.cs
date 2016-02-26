using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace MusicProject.Models
{
    public class MusicContext : DbContext
    {
        public MusicContext() : base("name = MusicContext")
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Composer> Composers { get; set; }
        public DbSet<Song> Songs { get; set; }

        //The modelBuilder.Conventions.Remove statement in the OnModelCreating method prevents table names from being pluralized. 
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            
            //one album has many songs
           /* modelBuilder.Entity<Album>()                   
                .HasMany<Song>(s => s.Songs)
                .WithOptional(s => s.Albums)
                .HasForeignKey(s => s.AlbumID);

            one artist has many songs and albums
            modelBuilder.Entity<Artist>()
                .HasMany<Song>(s => s.Songs)                                
                .WithRequired(s => s.Artists)
                .HasForeignKey(s => s.ArtistID);

            modelBuilder.Entity<Artist>()
                .HasMany<Album>(s => s.Albums)
                .WithRequired(s => s.Artists)
                .HasForeignKey(s => s.ArtistID); 

            
            modelBuilder.Entity<Artist>()
                .HasOptional<Company>(s => s.Companies)
                .WithMany(s => s.Artists)
                .HasForeignKey(s => s.CompanyID);

            modelBuilder.Entity<Composer>()
                .HasOptional<Company>(s => s.Companies)
                .WithMany(s => s.Composers)
                .HasForeignKey(s => s.ConmpanyID);
                */
        }

    }
}