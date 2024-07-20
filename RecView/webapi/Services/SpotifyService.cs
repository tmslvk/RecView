using Newtonsoft.Json;
using webapi.DTO;
using webapi.Interfaces;
using webapi.Model;

namespace webapi.Services
{
    public class SpotifyService : ISpotifyService
    {
        private readonly HttpClient _httpClient;
        private const string baseUrl = "https://api.spotify.com/v1/";

        public SpotifyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(baseUrl);
        }

        public async Task<AlbumDTO> GetAlbum(string albumId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"albums/{albumId}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var albumDto = JsonConvert.DeserializeObject<AlbumDTO>(jsonString);
                return albumDto;
            }

            throw new Exception($"Failed to retrieve album with ID {albumId}. Status code: {response.StatusCode}");
        }

        public async Task<ArtistDTO> GetArtist(string artistId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"artists/{artistId}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var artistDto = JsonConvert.DeserializeObject<ArtistDTO>(jsonString);
                return artistDto;
            }

            throw new Exception($"Failed to retrieve artist with ID {artistId}. Status code: {response.StatusCode}");
        }

        public async Task<List<SongDTO>> GetSongsByAlbum(string albumId)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"artists/{albumId}/tracks");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var albumTracks = JsonConvert.DeserializeObject<AlbumTrackResponse>(jsonString);
                return albumTracks.Songs;
            }

            throw new Exception($"Failed to retrieve album with ID {albumId}. Status code: {response.StatusCode}");
        }

        public class AlbumTrackResponse
        {
            public List<SongDTO> Songs { get; set; }
        }
    }
}
