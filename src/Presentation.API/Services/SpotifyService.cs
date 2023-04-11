using Presentation.API.Models;
using System.Text.Json.Serialization;

namespace Presentation.API.Services
{
    public class SpotifyService : IMusicService
    {
        private record SpotifyTracksResponse(SpotifyTracks tracks);
        private record SpotifyTracks(IEnumerable<SpotifyTrack> items);
        private record SpotifyTrack(string id, string name, string preview_url, int duration_ms);

        private const string SpotifySearchUrl = "https://api.spotify.com/v1/search";

        public async Task<IEnumerable<Track>> findTracksAsync(string name)
        {
            var token = getToken();
            var url = $"{SpotifySearchUrl}?type=track&q=track:\"{name}\"";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetFromJsonAsync<SpotifyTracksResponse>(url);

            return response.tracks.items.Select(spotifyTrackToTrack);
        }

        private string getToken()
        {
            return "[TOKEN]";
        }

        private Track spotifyTrackToTrack(SpotifyTrack spotifyTrack)
        {
            return new Track
            {
                Id = spotifyTrack.id,
                Name = spotifyTrack.name,
                PreviewUrl = spotifyTrack.preview_url,
            };
        }
    }
}
