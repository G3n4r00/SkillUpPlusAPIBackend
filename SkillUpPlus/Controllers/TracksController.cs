using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia a exibição de Trilhas de Aprendizado (LRN) e seus Módulos.
    /// (Requer autenticação)
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        /// <summary>
        /// Busca o catálogo de todas as trilhas disponíveis, com filtro opcional por categoria.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna uma lista *resumida* das trilhas (sem o conteúdo dos módulos).
        /// (Atende ao requisito RF-005)
        /// </remarks>
        /// <param name="category">Filtro opcional. Ex: "Soft Skills", "Inteligência Artificial"</param>
        /// <response code="200">Retorna a lista de trilhas (resumida).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TrackSummaryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<TrackSummaryDto>>> GetTracks([FromQuery] string? category)
        {
            var tracks = await _trackService.GetAllTracksAsync(category);
            return Ok(tracks);
        }

        /// <summary>
        /// Busca os detalhes completos de uma única trilha, incluindo todos os seus módulos e conteúdos.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna a lista *completa* de módulos com seus conteúdos.
        /// (Atende aos requisitos RF-006 e RF-008)
        /// </remarks>
        /// <param name="id">O ID da trilha a ser buscada.</param>
        /// <response code="200">Retorna os detalhes completos da trilha.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        /// <response code="404">Nenhuma trilha com este ID foi encontrada.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TrackDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
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
