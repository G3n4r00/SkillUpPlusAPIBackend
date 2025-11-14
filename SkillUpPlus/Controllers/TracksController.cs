using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //ATENÇÃO TIRAR ISSO DEPOIS O COMENTADO
    // [Authorize]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        // GET: api/tracks?category=IA
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrackSummaryDto>>> GetTracks([FromQuery] string? category)
        {
            var tracks = await _trackService.GetAllTracksAsync(category);
            return Ok(tracks);
        }

        // GET: api/tracks/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TrackDetailDto>> GetTrack(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id);

            if (track == null)
            {
                return NotFound(new { message = "Trilha não encontrada." });
            }

            return Ok(track);
        }
    }
}
