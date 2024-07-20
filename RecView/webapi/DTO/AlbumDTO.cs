using Microsoft.Extensions.Diagnostics.HealthChecks;
using webapi.Model;

namespace webapi.DTO
{
    public class AlbumDTO
    {
        public Artist Artist { get; set; }
        public string ArtistId { get; set; }
        public string Title { get; set; }
        public string AlbumType { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<Song> Songs { get; set; }
        public List<UserOverview>? UserOverviews { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
