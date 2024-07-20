using webapi.DTO;

namespace webapi.Interfaces
{
    public interface ISpotifyService
    {
        Task<AlbumDTO> GetAlbum(string albumId);
        Task<ArtistDTO> GetArtist(string artistId);
        Task<List<SongDTO>> GetSongsByAlbum(string albumId);
    }
}
