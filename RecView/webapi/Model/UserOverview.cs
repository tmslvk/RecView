using System.Formats.Asn1;
using webapi.Models;

namespace webapi.Model
{
    public class UserOverview
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public User Author { get; set; }
        public Album OverviewedAlbum { get; set; }
        public int UserId { get; set; }
        public int AlbumId { get; set; }
        public uint Rating { get; set; }
    }
}
