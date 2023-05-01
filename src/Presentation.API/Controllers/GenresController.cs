using Microsoft.AspNetCore.Mvc;
using Presentation.API.Models;
using Presentation.API.Services;

namespace Presentation.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenresController : ControllerBase
    {
        IMusicService _musicService;

        public GenresController(IMusicService musicService)
        {
            _musicService = musicService;
        }

        [HttpGet]
        public Task<IEnumerable<Genre>> GetGenresAsync()
        {
            return _musicService.FindGenresAsync();
        }
    }
}
