using Newtonsoft.Json;
using RestSharp;
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

        public SpotifyUserInfo GetUserInfo(string accessToken)
        {
            // Получение информации о пользователе Spotify
            var client = new RestClient("https://api.spotify.com/v1/me");
            var request = new RestRequest("", Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessToken);

            RestResponse response = client.Execute(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<SpotifyUserInfo>(response.Content);
            }
            else
            {
                throw new ApplicationException("Failed to get user info from Spotify");
            }
        }

        public class SpotifyUserInfo
        {
            public string id;
            public string display_name;
            public string email;
            public string country;
        } 
    }
}
