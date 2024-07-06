using webapi.Model;

namespace webapi.DTO
{
    public class PublicationDTO
    {
        public DateTime PublicationTime { get; set; }
        public int OverviewId { get; set; }
        public UserOverview UserOverview { get; set; }
    }
}
