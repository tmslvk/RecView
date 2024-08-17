using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using RestSharp;
using SpotifyAPI.Web;
using System.Runtime.ExceptionServices;
using webapi.DTO;
using webapi.Models;

namespace webapi.Services
{
    public class UserService
    {
        ApplicationContext db;
        private readonly IMemoryCache _memoryCache;

        private const string AccessTokenCacheKey = "SpotifyAccessToken";
        private const string RefreshTokenCacheKey = "SpotifyRefreshToken";

        public UserService(ApplicationContext context, IMemoryCache memoryCache)
        {
            this.db = context;
            this._memoryCache = memoryCache;
        }

        public async Task<User> Add(UserRegDTO userDTO)
        {
            var spotifyId = userDTO.SpotifyId;
            if (userDTO.SpotifyId == "" || userDTO.SpotifyId == string.Empty)
            {
                spotifyId = null;
            }
            
            var user = new User()
            {
                Lastname = userDTO.Lastname,
                Firstname = userDTO.Firstname,
                Email = userDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password),
                Country = userDTO.Country,
                Username = userDTO.Username,
                SpotifyUserId = spotifyId
            };
            await db.AddAsync(user);
            await db.SaveChangesAsync();

            return user;
        }

        public async Task<User?> Login(UserLogDTO loginDto)
        {
            User? user = await db.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email || u.Username == loginDto.Username);
            if (user == null)
            {
                return null;
            }
            bool verified = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.Password);
            if (!verified)
            {
                return null;
            }
            db.Entry(user);
            return user;
        }

        public async Task<User?> GetUserBySpotifyId(string spotifyId)
        {
            return await db.Users.FirstOrDefaultAsync(u => u.SpotifyUserId == spotifyId);
        }

        public async Task<User?> GetOne(int id)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
                db.Entry(user);
            return user;
        }

        public async Task<bool> IsUserExists(string spotifyId)
        {
            var isExists = await db.Users.AnyAsync(su => su.SpotifyUserId == spotifyId);
            return isExists;
        }

        public async Task<bool> CheckUsername(string username)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Username == username);
            return user != null;
        }
        public async Task<bool> CheckEmail(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user != null;
        }

        public async Task<SpotifyUserInfo> GetUserInfo(string accessToken)
        {
            // Получение информации о пользователе Spotify
            var client = new RestClient("https://api.spotify.com/v1/me");
            var request = new RestRequest("", Method.Get);
            request.AddHeader("Authorization", "Bearer " + accessToken);

            try
            {
                RestResponse response = await client.ExecuteAsync(request);
                if (response.IsSuccessful)
                {
                    return JsonConvert.DeserializeObject<SpotifyUserInfo>(response.Content);
                }
                else
                {
                    throw new ApplicationException("Failed to get user info from Spotify");
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while getting user info from Spotify", ex);
            }
        }

        public string? GetAccessToken()
        {
            return _memoryCache.TryGetValue(AccessTokenCacheKey, out string accessToken) ? accessToken : null;
        }

        public string? GetRefreshToken()
        {
            return _memoryCache.TryGetValue(RefreshTokenCacheKey, out string refreshToken) ? refreshToken : null;
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
