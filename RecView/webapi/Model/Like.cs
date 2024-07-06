using webapi.Models;

namespace webapi.Model
{
    public class Like
    {
        public int Id { get; set; }
        public User User { get; set; }
        public int UserId { get; set; } 
        public int ObjectId { get; set; } 
        public bool IsLiked { get; set; } 
    }
}
