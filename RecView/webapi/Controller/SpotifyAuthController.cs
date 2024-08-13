using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using NetTopologySuite.Simplify;
using Newtonsoft.Json;
using RestSharp;
using SpotifyAPI.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using webapi.DTO;
using webapi.Generators;
using webapi.Model;
using webapi.Models;
using webapi.Services;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAuthController : ControllerBase
    {
        UserService userService;
        IConfiguration config;
        private readonly IMemoryCache memoryCache;

        public SpotifyAuthController (IConfiguration configuration, UserService userService, IMemoryCache memoryCache)
        {
            config = configuration;
            this.userService = userService; 
            this.memoryCache = memoryCache;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Redirect the user to Spotify's authorization page
            var scopes = new List<string> { 
                "user-read-private", 
                "user-read-email", 
                "user-follow-read",
                "user-read-recently-played",  
                "user-top-read",
            };
            var authorizeUrl = $"https://accounts.spotify.com/authorize" +
                $"?client_id={config["Spotify:CLIENT_ID"]}" +
                $"&response_type=code" +
                $"&redirect_uri={Uri.EscapeDataString(config["Spotify:CALLBACK"])}" +
                $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}";

                return Redirect(authorizeUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            try
            {
                var tokenResponse = await ExchangeCodeForToken(code);
                if (tokenResponse == null)
                {
                    return BadRequest("Unable to retrieve access token.");
                }
                var accessToken = tokenResponse.access_token;
                var refreshToken = tokenResponse.refresh_token;

                var userInfo = await userService.GetUserInfo(accessToken);

                memoryCache.Set("SpotifyAccessToken", accessToken, DateTime.UtcNow.AddHours(1));
                memoryCache.Set("SpotifyRefreshToken", refreshToken, DateTime.UtcNow.AddHours(1));
                
                var jwtToken = GenerateJwtToken(userInfo.display_name, userInfo.id, userInfo.email, userInfo.country);
                if (!await userService.IsUserExists(userInfo.id))
                {
                    var url = $"https://localhost:5173/register?userId={userInfo.id}&displayName={Uri.EscapeDataString(userInfo.display_name)}&email={Uri.EscapeDataString(userInfo.email)}&country={Uri.EscapeDataString(userInfo.country)}";
                    return Redirect(url);
                }

                return RedirectToAction("RedirectToClient", new { token = jwtToken });

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("redirect-to-client")]
        public IActionResult RedirectToClient(string token)
        {
            // The URL to redirect to
            var redirectUrl = $"https://localhost:5173/callback?token={token}";
            memoryCache.Set("JWTToken", token, DateTime.UtcNow.AddHours(1));
            return Redirect(redirectUrl);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegDTO userRegDTO)
        {
            try
            {
                if(userRegDTO == null)
                {
                    return NoContent();
                }
                await userService.Add(userRegDTO);

                var token = GenerateJwtToken(userRegDTO.Username, userRegDTO.SpotifyId, userRegDTO.Email, userRegDTO.Country);
                var redirectUrl = $"https://localhost:5173/callback";
                memoryCache.Set("JWTToken", token, DateTime.UtcNow.AddHours(1));
                return Created(redirectUrl, token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<TokenResponse> ExchangeCodeForToken(string code)
        {
            var client = new RestClient("https://accounts.spotify.com/api/token");
            RestRequest request = new RestRequest("", Method.Post);
            var clientId = config["Spotify:CLIENT_ID"];
            var clientSecret = config["Spotify:CLIENT_SECRET"];
            var redirectUri = config["Spotify:REDIRECT_URI"];
            var callback = config["Spotify:CALLBACK"];
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", callback);

            var response = await client.ExecuteAsync<TokenResponse>(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TokenResponse>(response.Content);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                return null;
            }
        }

        [HttpGet("refresh")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            // Проверяем наличие access token в кеше
            if (!memoryCache.TryGetValue("SpotifyRefreshToken", out string refreshToken))
            {
                return BadRequest("Access token is missing. User must be authorized.");
            }

            var tokenResponse = await ExchangeRefreshTokenForAccessToken(refreshToken);
            if (tokenResponse == null)
            {
                return BadRequest("Failed to refresh access token.");
            }
            var newAccessToken = tokenResponse.access_token;
            memoryCache.Set("SpotifyAccessToken", newAccessToken, TimeSpan.FromHours(1));
           

            return Ok(new { AccessToken = newAccessToken });
        }
        private async Task<TokenResponse> ExchangeRefreshTokenForAccessToken(string refreshToken)
        {
            var client = new RestClient("https://accounts.spotify.com/api/token");
            var request = new RestRequest("", Method.Post);
            var clientId = config["Spotify:CLIENT_ID"];
            var clientSecret = config["Spotify:CLIENT_SECRET"];
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("grant_type", "refresh_token");
            request.AddParameter("refresh_token", refreshToken);

            var response = await client.ExecuteAsync<TokenResponse>(request);
            if (response.IsSuccessful)
            {
                return JsonConvert.DeserializeObject<TokenResponse>(response.Content);
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                return null;
            }
        }

        [HttpGet("token")]
        public IActionResult GetResourceToken()
        {
            var token = memoryCache.Get("JWTToken");
            return Ok(token);
        }

        [HttpGet("spotify-tokens")]
        public IActionResult GetTokens()
        {
            var accessToken = userService.GetAccessToken();
            var refreshToken = userService.GetRefreshToken();

            if(accessToken == null || refreshToken == null)
            {
                return NotFound("Tokens are not available.");
            }

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        private string GenerateJwtToken(string displayName, string id, string email, string country)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:SECRET_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Country, country),
                new Claim(ClaimTypes.Name, displayName)
            };
            var token = new JwtSecurityToken(
                issuer: config["JWT:ISSUER"],
                audience: config["JWT:AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public class AdditionalUserInfo
        {
            public string Firstname { get; set; }
            public string Lastname { get; set; }
            public string Password { get; set; }
        }

        public class SpotifyInfo
        {
            public string SpotifyId { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Country { get; set; }
        }
        private class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
        }
    }
}
