using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using webapi.DTO;
using webapi.Model;
using webapi.Services;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpotifyAuthController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        SpotifyUserService service;
        public SpotifyAuthController(IHttpClientFactory httpClientFactory, SpotifyUserService spotifyUserService)
        {
            _httpClientFactory = httpClientFactory;
            service = spotifyUserService;
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            var redirectUrl = Url.Action(nameof(HandleSpotifyResponse), "SpotifyAuth", null, Request.Scheme);
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, "Spotify");
        }

        [HttpGet("handle-spotify-response")]
        public async Task<IActionResult> HandleSpotifyResponse()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync("Spotify");

            if (!authenticateResult.Succeeded)
            {
                return BadRequest("Authentication failed.");
            }

            // Получение информации о пользователе от Spotify API
            var accessToken = authenticateResult.Properties.GetTokenValue("access_token");
            var userInfo = await GetSpotifyUserInfoAsync(accessToken);

            // Обработка информации о пользователе
            var newuser = new SpotifyRegDTO()
            {
                Country = userInfo.Country,
                Email = userInfo.Email,
                Username = userInfo.DisplayName
            };

            // Здесь вы можете добавить логику сохранения или обновления пользователя в вашей базе данных
            var user = await service.Add(newuser);
            // или создания JWT токена и отправки его клиенту

            return Ok(user);
        }

        private async Task<SpotifyUser> GetSpotifyUserInfoAsync(string accessToken)
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.GetAsync("https://api.spotify.com/v1/me");
            response.EnsureSuccessStatusCode();

            using var responseStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<SpotifyUser>(responseStream);
        }
    }
}
