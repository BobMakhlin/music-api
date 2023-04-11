using Presentation.API.Models;
using System.Text.Json.Serialization;

namespace Presentation.API.Services
{
    public class SpotifyService : IMusicService
    {
        private record SpotifyTracksResponse(SpotifyTracks tracks);
        private record SpotifyTracks(IEnumerable<SpotifyTrack> items);
        private record SpotifyTrack(string id, string name, string preview_url,
            int duration_ms, ExternalUrls external_urls);
        private record ExternalUrls(string spotify);

        private const string SpotifySearchUrl = "https://api.spotify.com/v1/search";

        public async Task<IEnumerable<Track>> FindTracksAsync(string name)
        {
            var token = GetToken();
            var url = $"{SpotifySearchUrl}?type=track&q=track:\"{name}\"";

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var response = await client.GetFromJsonAsync<SpotifyTracksResponse>(url);

            return response.tracks.items.Select(SpotifyTrackToTrack);
        }

        private string GetToken()
        {
            return "BQBVf1r8iVYxMh3a9yNwICsk5AMHw0W4dS25YrDGrn5IQAs-XKnedntGqr2Dk1j42ygXD7l0roqlnOkukaAfiBVvpk_PSsz6x13aaq09rbV9LuAS12B2";
        }

        private Track SpotifyTrackToTrack(SpotifyTrack spotifyTrack)
        {
            return new Track(
                spotifyTrack.id,
                spotifyTrack.name,
                spotifyTrack.preview_url,
                null,
                null,
                spotifyTrack.external_urls.spotify
            );
        }
    }
}
