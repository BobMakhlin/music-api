using Microsoft.Extensions.Caching.Memory;
using Presentation.API.Models;

namespace Presentation.API.Services
{
    public class SpotifyService : IMusicService
    {
        private record SpotifyTracksResponse(SpotifyTracks tracks);
        private record SpotifyTracks(IEnumerable<SpotifyTrack> items);
        private record SpotifyTrack(string id, string name, string preview_url,
            int duration_ms, SpotifyExternalUrls external_urls, SpotifyAlbumImages album);
        private record SpotifyExternalUrls(string spotify);
        private record SpotifyAlbumImages(IEnumerable<SpotifyImage> images);
        private record SpotifyImage(string url, int height, int width);
        private record TokenResponse(string access_token, string token_type, int expires_in);
        private record SpotifyArtistsResponse(SpotifyArtists artists);
        private record SpotifyArtists(IEnumerable<SpotifyArtist> items);
        private record SpotifyArtist(string id, string name, int popularity,
            SpotifyExternalUrls external_urls);
        private record SpotifyAlbumsResponse(SpotifyAlbums albums);
        private record SpotifyAlbums(IEnumerable<SpotifyAlbum> items);
        private record SpotifyAlbum(string id, string name, SpotifyExternalUrls external_urls);
        private record SpotifyGenresResponse(IEnumerable<string> genres);

        private const string SearchPath = "search";
        private const string TokenPath = "token";
        private const string GenresPath = "recommendations/available-genre-seeds";
        private const int Small = 64;
        private const string AccessTokenCacheKey = "spotifyAccessToken";
        private const int AccessTokenCorrelationMinutes = 5;

        private readonly IConfiguration _configuration;
        private readonly HttpClient _spotifyHttpClient;
        private readonly HttpClient _accountsHttpClient;
        private readonly IMemoryCache _cache;

        public SpotifyService(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache)
        {
            _configuration = configuration;
            _cache = cache;
            _spotifyHttpClient = httpClientFactory.CreateClient();
            _spotifyHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:SpotifyApi"]);
            _accountsHttpClient = httpClientFactory.CreateClient();
            _accountsHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:AccountsApi"]);
        }

        public async Task<IEnumerable<Track>> FindTracksAsync(string name)
        {
            var token = await GetTokenAsync();
            var url = $"{SearchPath}?type=track&q=track:\"{name}\"";

            _spotifyHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            var response = await _spotifyHttpClient.GetFromJsonAsync<SpotifyTracksResponse>(url);

            return response.tracks.items.Select(SpotifyTrackToTrack);
        }

        public async Task<IEnumerable<Artist>> FindArtistsAsync(string name)
        {
            var token = await GetTokenAsync();
            var url = $"{SearchPath}?type=artist&q=artist:\"{name}\"";

            _spotifyHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            var response = await _spotifyHttpClient.GetFromJsonAsync<SpotifyArtistsResponse>(url);

            return response.artists.items.Select(SpotifyArtistToArtist);
        }

        public async Task<IEnumerable<Album>> FindAlbumsAsync(string name)
        {
            var token = await GetTokenAsync();
            var url = $"{SearchPath}?type=album&q=album:\"{name}\"";

            _spotifyHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            var response = await _spotifyHttpClient.GetFromJsonAsync<SpotifyAlbumsResponse>(url);

            return response.albums.items.Select(SpotifyAlbumToAlbum);
        }

        public async Task<IEnumerable<Genre>> FindGenresAsync()
        {
            var token = await GetTokenAsync();

            _spotifyHttpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            var response = await _spotifyHttpClient.GetFromJsonAsync<SpotifyGenresResponse>(GenresPath);

            return response.genres.Select(SpotifyGenreToGenre);
        }

        private async Task<string> GetTokenAsync()
        {
            string token;

            if (!_cache.TryGetValue(AccessTokenCacheKey, out token))
            {
                var map = new Dictionary<string, string>
                {
                    { "grant_type", "client_credentials" },
                    { "client_id", _configuration["ThirdParty:Spotify:ClientId"] },
                    { "client_secret", _configuration["ThirdParty:Spotify:ClientSecret"]}
                };
                var response = await _accountsHttpClient.PostAsync($"{TokenPath}", new FormUrlEncodedContent(map));
                var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

                var absoluteExpiration = TimeSpan.FromSeconds(content.expires_in - AccessTokenCorrelationMinutes);
                _cache.Set(AccessTokenCacheKey, content.access_token, absoluteExpiration);
                token = content.access_token;
            }

            return $"Bearer {token}";
        }

        private Track SpotifyTrackToTrack(SpotifyTrack spotifyTrack)
        {
            var albumImage = spotifyTrack.album.images.First(image =>
                image.height == Small && image.width == Small);
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
        private Artist SpotifyArtistToArtist(SpotifyArtist spotifyArtist)
        {
            return new Artist(spotifyArtist.id, spotifyArtist.name, spotifyArtist.popularity,
                spotifyArtist.external_urls.spotify);
        }
        private Album SpotifyAlbumToAlbum(SpotifyAlbum spotifyAlbum)
        {
            return new Album(spotifyAlbum.id, spotifyAlbum.name,
                spotifyAlbum.external_urls.spotify);
        }
        private Genre SpotifyGenreToGenre(string spotifyGenre)
        {
            return new Genre(spotifyGenre);
        }
    }
}
