using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia o leaderboard de gamificação (XP).
    /// </summary>
    /// <remarks>
    /// Este controller é **exclusivo da v2.0** e implementa a
    /// funcionalidade de "Gamificação Avançada",
    /// fornecendo o ranking de usuários.
    /// </remarks>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/leaderboard")]
    [Authorize]
    [Produces("application/json")]
    public class LeaderboardController : ControllerBase
    {
        private readonly IProgressService _progressService;

        /// <summary>
        /// Inicializa uma nova instância do LeaderboardController.
        /// </summary>
        /// <param name="progressService">O serviço de progresso (que contém a lógica do leaderboard).</param>
        public LeaderboardController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Busca o Top 10 de usuários da plataforma, ordenados por XP.
        /// </summary>
        /// <remarks>
        /// Retorna uma lista ranqueada de usuários. O critério de
        /// ordenação principal é o XP total, com o número de badges
        /// como critério de desempate.
        /// </remarks>
        /// <response code="200">Retorna a lista do Top 10 (Leaderboard).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LeaderboardEntryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<LeaderboardEntryDto>>> GetLeaderboard()
        {
            var leaderboard = await _progressService.GetLeaderboardAsync();
            return Ok(leaderboard);
        }
    }
}
