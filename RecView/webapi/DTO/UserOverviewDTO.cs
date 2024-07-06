using webapi.Model;
using webapi.Models;

namespace webapi.DTO
{
    public class UserOverviewDTO
    { 
        public string Title { get; set; }
        public string Description { get; set; }
        public User Author { get; set; }
        public Album Album { get; set; }
        public int UserId { get; set; }
        public int AlbumId { get; set; }
        public uint Rating { get; set; }
    }
}
