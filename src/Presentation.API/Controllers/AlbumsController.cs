using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumsController : ControllerBase
    {
        IMusicService _musicService;

        public AlbumsController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public Task<IEnumerable<Album>> GetAlbumsAsync([FromQuery] string name)
        {
            return _musicService.FindAlbumsAsync(name);
        }
    }
}
