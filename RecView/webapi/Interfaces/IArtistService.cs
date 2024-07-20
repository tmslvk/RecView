using webapi.Model;

namespace webapi.Interfaces
{
    public interface IArtistService
    {
        Task<Artist> Add(string artistId);
    }
}
