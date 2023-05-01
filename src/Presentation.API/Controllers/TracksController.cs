using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

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
        public Task<IEnumerable<Track>> GetTracksAsync([FromQuery] FilterTracksQuery query)
        {
            return _musicService.FindTracksAsync(query);
        }
    }
}
