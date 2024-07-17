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
        public DbSet<SpotifyUser> SpotifyUsers { get; set; }
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
                .HasForeignKey(uo => uo.UserId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade); ;

            //Artist-album one-to-many
            modelBuilder.Entity<Artist>()
                .HasMany(a => a.Albums)
                .WithOne(album => album.Artist)
                .HasForeignKey(album => album.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            //Album-song one-to-many 
            modelBuilder.Entity<Album>()
                .HasMany(album => album.Songs)
                .WithOne(song => song.Album)
                .HasForeignKey(song => song.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);

            //Publication-UserOverview one-to-one
            modelBuilder.Entity<Publication>()
                .HasOne(p => p.UserOverview)
                .WithOne()
                .HasForeignKey<Publication>(p => p.Id)
                .OnDelete(DeleteBehavior.Cascade);

            //UserOverview-Album one-to-many
            modelBuilder.Entity<UserOverview>()
                .HasOne(uo => uo.OverviewedAlbum)
                .WithMany(a => a.UserOverviews)
                .HasForeignKey(uo => uo.AlbumId)
                .OnDelete(DeleteBehavior.Restrict);

            //Like
            modelBuilder.Entity<Like>()
                .HasKey(l => l.Id);

            //User-SpotifyUser
            modelBuilder.Entity<User>()
            .HasOne(u => u.SpotifyUser)
            .WithOne()
            .HasForeignKey<User>(u => u.SpotifyUserId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
