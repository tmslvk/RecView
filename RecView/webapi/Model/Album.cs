namespace webapi.Model
{
    public class Album
    {
        public int Id { get; set; }
        public Artist Artist { get; set; }
        public int ArtistId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<Song> Songs { get; set; }
        public List<UserOverview> UserOverviews { get; set; }
    }
}
