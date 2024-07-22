using Microsoft.EntityFrameworkCore;
using webapi.DTO;
using webapi.Model;

namespace webapi.Services
{
    public class UserOverviewService
    {
        ApplicationContext db;

        public UserOverviewService(ApplicationContext context)
        {
            this.db = context;
        }

        public async Task<UserOverview> Add(UserOverviewDTO userOverviewDTO)
        {
            var userOverview = new UserOverview()
            {
                Author = userOverviewDTO.Author,
                Description = userOverviewDTO.Description,
                Rating = userOverviewDTO.Rating,
                Title = userOverviewDTO.Title,
                UserId = userOverviewDTO.UserId,
                AlbumId = userOverviewDTO.AlbumId,
                OverviewedAlbum = userOverviewDTO.Album,
            };

            await db.AddAsync(userOverview);
            await db.SaveChangesAsync();

            return userOverview;
        }

        public async Task<UserOverview?> Get(int id)
        {
            return await db.UserOverviews.FirstOrDefaultAsync(uo => uo.Id == id);
        }

        public async Task<List<UserOverview>?> GetByAuthor(int userId)
        {
            return await db.UserOverviews.Where(uo=>uo.UserId == userId).ToListAsync();
        }

        public async Task<List<UserOverview>?> GetByAlbum(string albumId)
        {
            return await db.UserOverviews.Where(uo=>uo.AlbumId == albumId).ToListAsync();
        }

        public async Task<List<UserOverview>?> GetByArtist(string artistId)
        {
            return await db.UserOverviews.Where(uo => uo.OverviewedAlbum.ArtistId == artistId).ToListAsync();
        }
    }
}
