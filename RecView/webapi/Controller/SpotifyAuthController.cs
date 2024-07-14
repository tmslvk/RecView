using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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
        IConfiguration config;
        public SpotifyAuthController (SpotifyUserService spotifyUserService, IConfiguration configuration)
        {
            service = spotifyUserService;
            config = configuration;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            // Redirect the user to Spotify's authorization page
            var scopes = new List<string> { 
                "user-read-private", 
                "user-read-email", 
                //"user-follow-read",
                //"user-read-recently-played",  
                //"user-top-read",
            };
            var authorizeUrl = $"https://accounts.spotify.com/authorize" +
                $"?client_id={config["Spotify:CLIENT_ID"]}" +
                $"&response_type=code" +
                $"&redirect_uri={Uri.EscapeDataString(config["Spotify:REDIRECT_URI"])}" +
                $"&scope={Uri.EscapeDataString(string.Join(" ", scopes))}";

            return Redirect(authorizeUrl);
        }

        [HttpGet("callback")]
        public async Task<IActionResult> Callback(string code)
        {
            // Exchange the authorization code for an access token
            var token = await ExchangeCodeForToken(code);
            if (token == null)
            {
                return BadRequest("Unable to retrieve access token.");
            }

            var jwtToken = GenerateJwtToken();

            return Ok(new { Token = jwtToken });
        }

        private async Task<string> ExchangeCodeForToken(string code)
        {
            var client = new RestClient("https://accounts.spotify.com/api/token");
            RestRequest request = new RestRequest("POST");
            var clientId = config["Spotify:CLIENT_ID"];
            var clientSecret = config["Spotify:CLIENT_SECRET"];
            var redirectUri = config["Spotify:REDIRECT_URI"];
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("client_id", clientId);
            request.AddParameter("client_secret", clientSecret);
            request.AddParameter("grant_type", "authorization_code");
            request.AddParameter("code", code);
            request.AddParameter("redirect_uri", redirectUri);

            var response = await client.ExecuteAsync<TokenResponse>(request);
            if (response.IsSuccessful)
            {
                return response.Data.AccessToken;
            }
            else
            {
                Console.WriteLine(response.ErrorMessage);
                return null;
            }
        }

        //private async Task<string> ExchangeCodeForToken(string code)
        //{
        //    var clientId = config["Spotify:CLIENT_ID"];
        //    var clientSecret = config["Spotify:CLIENT_SECRET"];
        //    var redirectUri = config["Spotify:REDIRECT_URI"];

        //    using (var httpClient = new HttpClient())
        //    {
        //        var requestBody = new List<KeyValuePair<string, string>>
        //    {
        //        new KeyValuePair<string, string>("client_id", clientId),
        //        new KeyValuePair<string, string>("client_secret", clientSecret),
        //        new KeyValuePair<string, string>("grant_type", "authorization_code"),
        //        new KeyValuePair<string, string>("code", code),
        //        new KeyValuePair<string, string>("redirect_uri", redirectUri)
        //    };

        //        var requestContent = new FormUrlEncodedContent(requestBody);
        //        var response = await httpClient.PostAsync("https://accounts.spotify.com/api/token", requestContent);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            var responseContent = await response.Content.ReadAsStringAsync();
        //            var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
        //            var jopa = tokenResponse.ToString();
        //            return tokenResponse.AccessToken;
        //        }
        //        else
        //        {
        //            // Handle error
        //            Console.WriteLine($"Failed to retrieve access token: {response.StatusCode}");
        //            return null;
        //        }
        //    }
        //}

        private string GenerateJwtToken()
        {
            var jwtSecretKey = config["Jwt:SECRET_KEY"];
            var jwtIssuer = config["Jwt:ISSUER"];
            var jwtAudience = config["Jwt:AUDIENCE"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: new[] { new Claim("scope", "api_access") },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private class TokenResponse
        {
            public string AccessToken { get; set; }
            public string TokenType { get; set; }
            public int ExpiresIn { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
