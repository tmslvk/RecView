using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SpotifyAPI.Web;
using webapi.DTO;
using webapi.Model;
using webapi.Services;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        SpotifyUserService _spotifyUserService;
        UserService _userService;
        UserOverviewService _userOverviewService;
        SpotifyService _spotifyService;
        AlbumService _albumService;
        ArtistService _artistService;

        public ProfileController(IConfiguration configuration, SpotifyUserService spotifyUserService, UserService userService, UserOverviewService userOverviewService, SpotifyService spotifyService, AlbumService albumService, ArtistService artistService)
        {
            _configuration = configuration;
            _spotifyUserService = spotifyUserService;
            _userService = userService;
            _userOverviewService = userOverviewService;
            _spotifyService = spotifyService;
            _albumService = albumService;
            _artistService = artistService;
        }

        [Authorize]
        [HttpPost("publication-overview")]
        public async Task<ActionResult<UserOverview>> AddOverview(UserOverviewDTO userOverview)
        {
            var userContext = HttpContext.User;
            int userID = int.Parse(userContext.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
            var user = await _userService.GetOne(userID);
            if (userOverview != null && user != null)
            {
                var overview = new UserOverviewDTO()
                {
                    UserId = userOverview.UserId,
                    Description = userOverview.Description,
                    Album = userOverview.Album,
                    Author = user,
                    Rating = userOverview.Rating,
                    Title = userOverview.Title,
                    AlbumId = userOverview.AlbumId
                };

                await _userOverviewService.Add(overview);
                return Created("", overview);
            }
            return BadRequest();
        }

        [HttpGet("album")]
        public async Task<ActionResult<AlbumDTO>> GetAlbum(string albumId)
        {
            var album = await _spotifyService.GetAlbum(albumId);

            if(album != null)
            {
                return Ok(album);
            }
            return BadRequest();
        }

        [HttpGet("artist")]
        public async Task<ActionResult<ArtistDTO>> GetArtist(string artistId)
        {
            var artist = await _spotifyService.GetArtist(artistId);
            if(artist != null)
            {
                return Ok(artist);
            }
            return BadRequest();
        }

        [HttpGet("song")]
        public async Task<ActionResult<SongDTO>> GetSongsByAlbum(string albumId)
        {
            var songs = await _spotifyService.GetSongsByAlbum(albumId);
            if (songs != null)
            {
                return Ok(songs);
            }
            return BadRequest();
        }

        [HttpPost("albums/{albumId}/add")]
        public async Task<ActionResult<Album>> AddToDB(Album album)
        {
            var newAlbum = _albumService.Add(album.Id);
            if(newAlbum != null)
            {
                return Created("", newAlbum);
            }
            return BadRequest();
        }

        [HttpPost("artists/{artistId}/add")]
        public async Task<ActionResult<Album>> AddToDB(Artist artist)
        {
            var newArtist = _artistService.Add(artist.Id);
            if(newArtist != null)
            {
                return Created("", newArtist);
            }
            return BadRequest();
        }
    }
}
