namespace Infrastructure.Music
{
    internal record SpotifyTracksResponse(SpotifyTracks tracks);
    internal record SpotifyTracks(IEnumerable<SpotifyTrack> items);
    internal record SpotifyTrack(string id, string name, string preview_url,
        int duration_ms, SpotifyExternalUrls external_urls, SpotifyAlbumImages album);
    internal record SpotifyExternalUrls(string spotify);
    internal record SpotifyAlbumImages(IEnumerable<SpotifyImage> images);
    internal record SpotifyImage(string url, int height, int width);
    internal record TokenResponse(string access_token, string token_type, int expires_in);
    internal record SpotifyArtistsResponse(SpotifyArtists artists);
    internal record SpotifyArtists(IEnumerable<SpotifyArtist> items);
    internal record SpotifyArtist(string id, string name, int popularity,
        SpotifyExternalUrls external_urls);
    internal record SpotifyAlbumsResponse(SpotifyAlbums albums);
    internal record SpotifyAlbums(IEnumerable<SpotifyAlbum> items);
    internal record SpotifyAlbum(string id, string name, SpotifyExternalUrls external_urls);
    internal record SpotifyGenresResponse(IEnumerable<string> genres);
}
