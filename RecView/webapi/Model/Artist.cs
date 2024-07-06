namespace webapi.Model
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Foundation { get; set; }
        public List<Album> Albums { get; set; }
    }
}
