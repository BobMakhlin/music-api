﻿using Presentation.API.Models;

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
        private record TokenResponse(string access_token, string token_type, int expires_in);

        private const string SearchPath = "search";
        private const string TokenPath = "token";
        private const int Small = 64;

        private readonly IConfiguration _configuration;
        private readonly HttpClient _spotifyHttpClient;
        private readonly HttpClient _accountsHttpClient;

        public SpotifyService(IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _spotifyHttpClient = httpClientFactory.CreateClient();
            _spotifyHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:SpotifyApi"]);
            _accountsHttpClient = httpClientFactory.CreateClient();
            _accountsHttpClient.BaseAddress = new Uri(_configuration["ThirdParty:Spotify:AccountsApi"]);
        }

        public async Task<IEnumerable<Track>> FindTracksAsync(string name)
        {
            var token = await GetTokenAsync();
            var url = $"{SearchPath}?type=track&q=track:\"{name}\"";

            _spotifyHttpClient.DefaultRequestHeaders.Add("Authorization", token);
            var response = await _spotifyHttpClient.GetFromJsonAsync<SpotifyTracksResponse>(url);

            return response.tracks.items.Select(SpotifyTrackToTrack);
        }

        private async Task<string> GetTokenAsync()
        {
            var map = new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" },
                { "client_id", _configuration["ThirdParty:Spotify:ClientId"] },
                { "client_secret", _configuration["ThirdParty:Spotify:ClientSecret"]}
            };

            var response = await _accountsHttpClient.PostAsync($"{TokenPath}", new FormUrlEncodedContent(map));
            var content = await response.Content.ReadFromJsonAsync<TokenResponse>();

            // TODO: caching.

            return $"Bearer {content.access_token}";
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
    }
}
