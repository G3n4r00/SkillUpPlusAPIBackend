using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;
using System.Security.Claims;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia o perfil do usuário, progresso de aprendizado e gamificação (badges, XP).
    /// </summary>
    /// <remarks>
    /// * **v1.0:** Lida com o progresso básico e concessão de badges.
    /// * **v2.0:** Adiciona a lógica de "Gamificação Avançada" (XP).
    /// (Requer autenticação)
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")] 
    [Route("api/v{version:apiVersion}/profile")]
    [Authorize]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProgressService _progressService;

        /// <summary>
        /// Inicializa uma nova instância do ProfileController.
        /// </summary>
        /// <param name="progressService">O serviço injetado que contém a lógica de progresso e gamificação.</param>
        public ProfileController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Marca um módulo de aprendizado como concluído para o usuário.
        /// </summary>
        /// <remarks>
        /// Registra o progresso e, se o módulo for o último de uma trilha,
        /// aciona a lógica para conceder um badge. Esta versão *não* retorna XP.
        /// </remarks>
        /// <param name="dto">Um objeto JSON contendo o ID (`ModuleId`) do módulo a ser marcado.</param>
        /// <response code="200">Módulo marcado com sucesso. Retorna o status da trilha e (opcionalmente) um novo badge.</response>
        /// <response code="400">Ocorreu um erro (ex: ID do módulo não existe). A mensagem de erro é retornada.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpPost("progress")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(ProgressResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProgressResponseDto>> MarkProgressV1([FromBody] MarkModuleDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var result = await _progressService.MarkModuleAsCompletedAsync(userId, dto.ModuleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca o painel (dashboard) do usuário (sem XP).
        /// </summary>
        /// <remarks>
        /// Endpoint da v1. Retorna o progresso, dados para "continuar",
        /// badges ganhos e recomendações. Esta versão *não* inclui XP total.
        /// </remarks>
        /// <response code="200">Retorna o objeto DashboardDto (v1) completo.</response>
        /// <response code="400">Ocorreu um erro ao buscar o perfil (ex: usuário não encontrado).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpGet("me")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardDto>> GetMyProfileV1()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                var dashboard = await _progressService.GetUserDashboardAsync(userId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Marca um módulo como concluído e retorna o XP ganho.
        /// </summary>
        /// <remarks>
        /// Endpoint da v2. Faz tudo da v1 (registra progresso, concede badge) e
        /// **adicionalmente** retorna os pontos de XP (Experiência) ganhos por este módulo.
        /// </remarks>
        /// <param name="dto">Um objeto JSON contendo o ID (`ModuleId`) do módulo a ser marcado.</param>
        /// <response code="200">Módulo marcado com sucesso. Retorna o status da trilha, badge (opcional) e o `xpGained`.</response>
        /// <response code="400">Ocorreu um erro (ex: ID do módulo não existe).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpPost("progress")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(ProgressResponseV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProgressResponseV2Dto>> MarkProgressV2([FromBody] MarkModuleDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                
                var result = await _progressService.MarkModuleAsCompletedV2Async(userId, dto.ModuleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca o painel (dashboard) do usuário, incluindo XP total.
        /// </summary>
        /// <remarks>
        /// Endpoint da v2. Retorna tudo do dashboard v1 e
        /// **adicionalmente** o `totalXp` (XP total) acumulado pelo usuário.
        /// </remarks>
        /// <response code="200">Retorna o objeto DashboardV2Dto (v2) completo, com XP.</response>
        /// <response code="400">Ocorreu um erro ao buscar o perfil (ex: usuário não encontrado).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpGet("me")]
        [MapToApiVersion("2.0")]
        [ProducesResponseType(typeof(DashboardV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardV2Dto>> GetMyProfileV2()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
               
                var dashboard = await _progressService.GetUserDashboardV2Async(userId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
