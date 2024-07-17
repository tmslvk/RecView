using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using webapi.Models;
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

        public ProfileController(IConfiguration configuration, SpotifyUserService spotifyUserService, UserService userService)
        {
            _configuration = configuration;
            _spotifyUserService = spotifyUserService;
            _userService = userService;
        }
    }
}
