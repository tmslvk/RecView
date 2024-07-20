using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using webapi.Model;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Http;
using NetTopologySuite.Simplify;
using webapi.DTO;
using webapi.Interfaces;

namespace webapi.Services
{
    public class ArtistService:IArtistService
    {
        ApplicationContext db;
        private readonly ISpotifyService _spotifyService;

        public ArtistService(ApplicationContext context, ISpotifyService spotifyService)
        {
            this.db = context;
            _spotifyService = spotifyService;
        }

        public async Task<List<Artist>> GetByName(string name)
        {
            return await db.Artists.Where(a => a.Name == name).ToListAsync();
        }
        
        public async Task<Artist> Add(string artistId)
        {
            var artistDTO = _spotifyService.GetArtist(artistId);
            var artist = new Artist()
            {
                Followers = artistDTO.Result.Followers,
                Albums = artistDTO.Result.Albums,
                Genres = artistDTO.Result.Genres,
                Name = artistDTO.Result.Name,
                Id = artistId,
            };

            await db.Artists.AddAsync(artist);
            await db.SaveChangesAsync();
            return artist;
        }
    }
}
