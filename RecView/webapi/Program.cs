using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Certificate;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using webapi;
using webapi.Models;
using webapi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddCors();
builder.Services.AddControllers()
        .AddJsonOptions(options =>
{
options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
//dependency injection
builder.Services.AddTransient<UserService>();
builder.Services.AddTransient<LikeService>();
builder.Services.AddTransient<UserOverviewService>();
builder.Services.AddTransient<PublicationService>();
builder.Services.AddTransient<SpotifyUserService>();
builder.Services.AddAuthentication(
        CertificateAuthenticationDefaults.AuthenticationScheme)
        .AddCertificate();
//through server
builder.Services.AddAuthentication("ApplicationJwtBearer")
    .AddJwtBearer("ApplicationJwtBearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ISSUER"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:AUDIENCE"],
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SECRET_KEY"])),
            ValidateIssuerSigningKey = true,
        };
    });

//through spotify
builder.Services.AddAuthentication("SpotifyJwtBearer")
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        // Конфигурация JWT Bearer
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["JWT:ISSUER"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:AUDIENCE"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SECRET_KEY"]))
        };
    })
    .AddOAuth("Spotify", options =>
    {
        options.ClientId = builder.Configuration["Spotify:CLIENT_ID"];
        options.ClientSecret = builder.Configuration["Spotify:CLIENT_SECRET"];
        options.CallbackPath = "/signin-spotify"; // Путь, на который Spotify отправит ответ после аутентификации
        options.AuthorizationEndpoint = "https://accounts.spotify.com/authorize";
        options.TokenEndpoint = "https://accounts.spotify.com/api/token";

        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "display_name");
        options.ClaimActions.MapJsonKey("urn:spotify:country", "country");

        options.Events = new OAuthEvents
        {
            OnCreatingTicket = context =>
            {
                var accessToken = context.AccessToken;
                // Дополнительная обработка токена, если необходимо
                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddHttpClient("Spotify", client =>
{
    client.BaseAddress = new Uri("https://api.spotify.com/");
});
var app = builder.Build();
app.UseRouting();
app.UseCors(options =>
{
options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.Map("/data", [Authorize] () => new { message = "Hello World!" });
app.MapControllers();

app.Run();

public class AuthOptions
{
    public const string ISSUER = "MyAuthServer";
    public const string AUDIENCE = "MyAuthClient";
    const string KEY = "mysupersecret_secretkey!123"; 
    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}