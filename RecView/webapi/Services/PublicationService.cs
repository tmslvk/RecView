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
    }
}
