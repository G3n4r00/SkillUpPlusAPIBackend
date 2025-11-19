using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia a exibição de Trilhas de Aprendizado (LRN) e seus Módulos.
    /// </summary>
    /// <remarks>
    /// Este controller serve como o "cardápio" da aplicação, permitindo
    /// ao usuário navegar pelas trilhas e ver seus conteúdos.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class TracksController : ControllerBase
    {
        private readonly ITrackService _trackService;

        /// <summary>
        /// Inicializa uma nova instância do TracksController.
        /// </summary>
        /// <param name="trackService">O serviço injetado que contém a lógica de busca de trilhas.</param>
        public TracksController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        /// <summary>
        /// Busca o catálogo de todas as trilhas disponíveis, com filtro opcional por categoria.
        /// </summary>
        /// <remarks>
        /// Este endpoint retorna uma lista *resumida*
        /// das trilhas (sem o conteúdo dos módulos). Se nenhuma trilha for encontrada,
        /// retorna uma lista vazia `[]`.
        /// </remarks>
        /// <param name="category">Filtro opcional. Ex: "Soft Skills", "Inteligência Artificial"</param>
        /// <response code="200">Retorna a lista de trilhas (resumida), que pode estar vazia.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
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
        /// <param name="id">O ID da trilha a ser buscada.</param>
        /// <response code="200">Retorna os detalhes completos da trilha.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        /// <response code="404">Nenhuma trilha com este ID foi encontrada.</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TrackDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TrackDetailDto>> GetTrack(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id);
            return Ok(track);
        }
    }
}
