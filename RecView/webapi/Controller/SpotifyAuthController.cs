using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using webapi.Model;
using webapi.Models;
using webapi.Services;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAuthController : ControllerBase
    {
        SpotifyUserService service;
        UserService userService;
        IConfiguration config;
        public SpotifyAuthController (SpotifyUserService spotifyUserService, IConfiguration configuration, UserService userService)
        {
            service = spotifyUserService;
            config = configuration;
            this.userService = userService; 
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
            var token = await ExchangeCodeForToken(code);
            if (token == null)
            {
                return BadRequest("Unable to retrieve access token.");
            }
            var userInfo = service.GetUserInfo(token);
            if (!service.IsUserExists(userInfo.id))
            {
                var spotifyUser = new SpotifyRegDTO()
                {
                    SpotifyId = userInfo.id,
                    Country = userInfo.country,
                    DisplayName = userInfo.display_name,
                    Email = userInfo.email
                };

                await service.Add(spotifyUser);
            }
            

            var jwtToken = GenerateJwtToken(userInfo.display_name, userInfo.id, userInfo.email, userInfo.country);

            return Ok(new { Token = userInfo });
        }

        private async Task<string> ExchangeCodeForToken(string code)
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
                
                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(response.Content.ToString());
                var access = token.access_token;
                
                return access;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                return null;
            }
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

        private class TokenResponse
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public int expires_in { get; set; }
            public string refresh_token { get; set; }
        }
    }
}
