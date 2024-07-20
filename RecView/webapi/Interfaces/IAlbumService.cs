using webapi.Model;

namespace webapi.Interfaces
{
    public interface IAlbumService
    {
        Task<Album> Add(string albumId);
    }
}
