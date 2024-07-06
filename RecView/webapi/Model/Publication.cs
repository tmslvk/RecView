using webapi.Models;

namespace webapi.Model
{
    public class Publication
    {
        public int Id { get; set; }
        public DateTime PublicationTime { get; set; }
        public int OverviewId { get; set; }
        public UserOverview UserOverview { get; set; }
        public int LikesCount { get; set; } = 0;
    }
}
