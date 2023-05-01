using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArtistsController : ControllerBase
    {
        IMusicService _musicService;

        public ArtistsController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public Task<IEnumerable<Artist>> GetArtistsAsync([FromQuery] string name)
        {
            return _musicService.FindArtistsAsync(name);
        }
    }
}
