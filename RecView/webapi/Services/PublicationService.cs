using Microsoft.EntityFrameworkCore;
using webapi.DTO;
using webapi.Model;

namespace webapi.Services
{
    public class PublicationService
    {
        ApplicationContext db;

        public PublicationService(ApplicationContext context)
        {
            this.db = context;
        }

        public async Task<Publication> Add(PublicationDTO publicationDTO)
        {
            var publication = new Publication()
            {
                PublicationTime = publicationDTO.PublicationTime,
                OverviewId = publicationDTO.OverviewId,
                UserOverview = publicationDTO.UserOverview,
            };

            await db.Publications.AddAsync(publication);
            await db.SaveChangesAsync();  

            return publication;
        }

        public async Task<List<Publication>> GetByAuthor(int userId)
        {
            return await db.Publications.Where(p=>p.UserOverview.UserId == userId).ToListAsync();
        }

        public async Task<List<Publication>> GetByLikesCount()
        {
            return await db.Publications.OrderByDescending(p=>p.LikesCount).ToListAsync();
        }

        public async Task<List<Publication>?> GetUserPostsByLikes(int userId)
        {
            return await db.Publications.Where(p => p.UserOverview.UserId == userId).OrderByDescending(p => p.LikesCount).ToListAsync();
        }

        public async Task<List<Publication>?> GetByAlbum(int albumId)
        {
            return await db.Publications.Where(p => p.UserOverview.AlbumId == albumId).ToListAsync();
        }

        public async Task<List<Publication>?> GetByOverall()
        {
            return await db.Publications.OrderByDescending(p=>p.UserOverview.Rating).ToListAsync();
        }

        public async Task<List<Publication>?> GetByOverall(int albumId)
        {
            return await db.Publications.Where(p=>p.UserOverview.AlbumId == albumId).OrderByDescending(p => p.UserOverview.Rating).ToListAsync();
        }

        public async Task<List<Publication>> GetUserPostsByOverall(int userId)
        {
            return await db.Publications.Where(p => p.UserOverview.UserId == userId).OrderByDescending(p => p.UserOverview.Rating).ToListAsync();
        }
    }
}
