using webapi.Model;

namespace webapi.DTO
{
    public class SongDTO
    {
        public string Title { get; set; }
        public string Duration { get; set; }
        public Album Album { get; set; }
        public string AlbumId { get; set; }
    }
}
