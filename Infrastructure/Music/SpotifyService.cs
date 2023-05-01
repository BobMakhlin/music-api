using Application.Interfaces;
using Application.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text;

namespace Infrastructure.Music
{
    public class SpotifyService : IMusicService
    {
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
        private readonly ILogger<SpotifyService> _logger;

        public SpotifyService(IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IMemoryCache cache,
            ILogger<SpotifyService> logger)
        {
            _configuration = configuration;
            _cache = cache;
            _logger = logger;
            _spotifyHttpClient = httpClientFactory.CreateClient();
            _spotifyHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:SpotifyApi"]);
            _accountsHttpClient = httpClientFactory.CreateClient();
            _accountsHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:AccountsApi"]);
        }

        public async Task<IEnumerable<Track>> FindTracksAsync(FilterTracksQuery query)
        {
            var token = await GetTokenAsync();
            var url = $"{SearchPath}?type=track&q={BuildSearchTracksQuery(query)}";
            _logger.LogDebug($"Sending request to Spotify, URL: {url}");

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
        private string BuildSearchTracksQuery(FilterTracksQuery query)
        {
            var builder = new StringBuilder();

            if (query.Name != null)
            {
                builder.Append($"track:\"{query.Name}\" ");
            }
            if (query.Album != null)
            {
                builder.Append($"album:\"{query.Album}\" ");
            }
            if (query.Artist != null)
            {
                builder.Append($"artist:\"{query.Artist}\" ");
            }
            if (query.Genre != null)
            {
                builder.Append($"genre:\"{query.Genre}\" ");
            }

            return builder.ToString();
        }
    }
}
