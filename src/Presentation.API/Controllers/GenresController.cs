using Application.Interfaces;
using Application.Models;
using Microsoft.AspNetCore.Mvc;

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
