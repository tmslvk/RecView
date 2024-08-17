using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SpotifyAPI.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.DTO;
using webapi.Models;
using webapi.Services;

namespace webapi.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        UserService service;

        public AuthController(IConfiguration configuration, UserService userService)
        {
            this.service = userService;
            _configuration = configuration;
        }
        #region[DefaultEntry]
        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserRegDTO request)
        {
            if ((await service.CheckUsername(request.Username)))
            {
                return BadRequest("This user already exists");
            }
            else if(await service.CheckEmail(request.Email))
            {
                return BadRequest("Email is already in use");
            }

            var user = await service.Add(request);
            var token = this.CreateToken((User)user);
            return Created("", token);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserLogDTO request)
        {
            if (request.Email == null || request.Email == String.Empty)
            {
                return BadRequest("Email cannot be empty");
            }

            if (request.Password == null || request.Password == string.Empty)
            {
                return BadRequest("Wrong password");
            }

            var user = await service.Login(request);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var token = CreateToken(user);
            return CreatedAtRoute("", token);
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<User>> Me()
        {
            var handler = new JwtSecurityTokenHandler();
            string? authHeader = HttpContext.Request.Headers.ContainsKey("Authorization") ? Request.Headers["Authorization"].ToString() : null;
            if (authHeader == null)
            {
                return BadRequest("No Authorization header provided");
            }

            authHeader = authHeader.Replace("Bearer ", "");
            var jsonToken = handler.ReadToken(authHeader);
            var tokenS = handler.ReadToken(authHeader) as JwtSecurityToken;

            if (tokenS == null)
            {
                return BadRequest("Bad token");
            }
            var id = tokenS.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;
            var user = await service.GetOne(int.Parse(id));
            if (user == null)
            {
                return BadRequest("User not found");
            }

            return user;
        }

        private string CreateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SECRET_KEY"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Country, user.Country),
                new Claim(ClaimTypes.Name, user.Username)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ISSUER"],
                audience: _configuration["JWT:AUDIENCE"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion
    }
}
