using Application.Models;

namespace Application.Interfaces
{
    public interface IMusicService
    {
        Task<IEnumerable<Track>> FindTracksAsync(FilterTracksQuery query);
        Task<IEnumerable<Artist>> FindArtistsAsync(string name);
        Task<IEnumerable<Album>> FindAlbumsAsync(string name);
        Task<IEnumerable<Genre>> FindGenresAsync();
    }
}
