namespace webapi.Model
{
    public class Song
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Duration { get; set; }
        public Album Album { get; set; }
        public string AlbumId { get; set; }
        public string SpotifyId { get; set; }
    }
}
