using webapi.Model;

namespace webapi.DTO
{
    public class ArtistDTO
    {
        public string Name { get; set; }
        public int Followers { get; set; }
        public List<Album> Albums { get; set; }
        public List<Genre> Genres { get; set; }
    }
}
