using Presentation.API.Models;

namespace Presentation.API.Services
{
    public interface IMusicService
    {
        Task<IEnumerable<Track>> findTracksAsync(string name);
    }
}
