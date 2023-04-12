using Presentation.API.Models;

namespace Presentation.API.Services
{
    public class SpotifyService : IMusicService
    {
        private record SpotifyTracksResponse(SpotifyTracks tracks);
        private record SpotifyTracks(IEnumerable<SpotifyTrack> items);
        private record SpotifyTrack(string id, string name, string preview_url,
            int duration_ms, SpotifyExternalUrls external_urls, SpotifyAlbum album);
        private record SpotifyExternalUrls(string spotify);
        private record SpotifyAlbum(IEnumerable<SpotifyImage> images);
        private record SpotifyImage(string url, int height, int width);

        private const string SpotifySearchUrl = "https://api.spotify.com/v1/search";
        private const int Small = 64;

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
            return "[TOKEN]";
        }

        private Track SpotifyTrackToTrack(SpotifyTrack spotifyTrack)
        {
            var albumImage = spotifyTrack.album.images.First(image =>
                image.height == Small || image.width == Small);
            var duration = TimeSpan.FromMilliseconds(spotifyTrack.duration_ms);

            return new Track(
                spotifyTrack.id,
                spotifyTrack.name,
                spotifyTrack.preview_url,
                albumImage.url,
                duration,
                spotifyTrack.external_urls.spotify
            );
        }
    }
}
