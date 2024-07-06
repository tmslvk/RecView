using Microsoft.EntityFrameworkCore;
using System;
using webapi.Model;
using webapi.Models;

namespace webapi
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Artist> Artists { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<Publication> Publications { get; set; }
        public DbSet<Song> Songs { get; set; } = null!;
        public DbSet<UserOverview> UserOverviews { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //User-UserOverview one-to-many
            modelBuilder.Entity<User>()
                .HasMany(u => u.Overviews)
                .WithOne(uo => uo.Author)
                .HasForeignKey(uo => uo.UserId).IsRequired(false);

            //Artist-album one-to-many
            modelBuilder.Entity<Artist>()
                .HasMany(a => a.Albums)
                .WithOne(album => album.Artist)
                .HasForeignKey(album => album.ArtistId);

            //Album-song one-to-many 
            modelBuilder.Entity<Album>()
                .HasMany(album => album.Songs)
                .WithOne(song => song.Album)
                .HasForeignKey(song => song.AlbumId);

            //Publication-UserOverview one-to-one
            modelBuilder.Entity<Publication>()
                .HasOne(p => p.UserOverview)
                .WithOne()
                .HasForeignKey<Publication>(p => p.Id);

            //UserOverview-Album one-to-many
            modelBuilder.Entity<UserOverview>()
                .HasOne(uo => uo.Album)
                .WithMany(a => a.UserOverviews)
                .HasForeignKey(uo => uo.Album);

            //Like
            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id);
        }
    }
}
