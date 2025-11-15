using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// (v2.0) Gerencia o leaderboard de gamificação (XP).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/leaderboard")]
    [Authorize]
    [Produces("application/json")]
    public class LeaderboardController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public LeaderboardController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Busca o Top 10 de usuários da plataforma, ordenados por XP.
        /// </summary>
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
