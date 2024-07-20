using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using webapi.DTO;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.Services
{
    public class AlbumService:IAlbumService
    {
        ApplicationContext db;
        private readonly ISpotifyService _spotifyService;

        public AlbumService(ApplicationContext context, ISpotifyService spotifyService)
        {
            this.db = context;
            this._spotifyService = spotifyService;
        }

        public async Task<List<Album>> GetByTitle(string Id)
        {
            return await db.Albums.Where(a => a.Id == Id).ToListAsync();
        }

        public async Task<Album> Add(string albumId)
        {
            var spotifyAlbum = _spotifyService.GetAlbum(albumId);
            var album = new Album()
            {
                Artist = spotifyAlbum.Result.Artist,
                ReleaseDate = spotifyAlbum.Result.ReleaseDate,
                Title = spotifyAlbum.Result.Title,
                Songs = spotifyAlbum.Result.Songs,
                ArtistId = spotifyAlbum.Result.ArtistId,
                Genres = spotifyAlbum.Result.Genres,
                AlbumType = spotifyAlbum.Result.AlbumType,
                Id = albumId,
            };

            await db.Albums.AddAsync(album);
            await db.SaveChangesAsync();

            return album;
        }


       
    }
}
