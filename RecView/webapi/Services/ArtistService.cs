using Microsoft.EntityFrameworkCore;
using SpotifyAPI.Web;
using webapi.Model;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Http;

namespace webapi.Services
{
    public class ArtistService
    {
        ApplicationContext db;
        APIConnector api;

        public ArtistService(ApplicationContext context)
        {
            this.db = context;
        }

        public async Task<List<Artist>> GetByName(string name)
        {
            return await db.Artists.Where(a => a.Name == name).ToListAsync();
        }

        public void Train()
        {
        }
    }
}
