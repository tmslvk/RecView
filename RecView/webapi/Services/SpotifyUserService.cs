using webapi.DTO;
using webapi.Model;

namespace webapi.Services
{
    public class SpotifyUserService
    {
        ApplicationContext db;

        public SpotifyUserService(ApplicationContext context)
        {
            db = context;
        }

        public async Task<SpotifyUser> Add(SpotifyRegDTO spotifyDto) 
        {
            var user = new SpotifyUser()
            {
                Country = spotifyDto.Country,
                DisplayName = spotifyDto.Username,
                Email = spotifyDto.Email,
            };

            
            await db.SpotifyUsers.AddAsync(user);
            await db.SaveChangesAsync();
            return user;
        }
    }
}
