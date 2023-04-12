using Microsoft.AspNetCore.Mvc;
using Presentation.API.Models;
using Presentation.API.Services;

namespace Presentation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TracksController : ControllerBase
    {
        IMusicService _musicService;

        public TracksController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public Task<IEnumerable<Track>> GetTracksAsync([FromQuery] string name)
        {
            return _musicService.FindTracksAsync(name);
        }
    }
}
