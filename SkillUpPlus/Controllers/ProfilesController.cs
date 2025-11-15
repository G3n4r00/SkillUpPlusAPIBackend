using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;
using System.Security.Claims;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia o perfil do usuário, progresso de aprendizado e gamificação (badges).
    /// (Requer autenticação)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProfileController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Marca um módulo de aprendizado como concluído para o usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Este é um endpoint crítico. Ele registra o progresso (RF-009) e,
        /// se o módulo for o último de uma trilha, aciona a lógica para conceder um badge (RF-012).
        /// </remarks>
        /// <param name="dto">Um objeto JSON contendo o ID do módulo a ser marcado.</param>
        /// <response code="200">Módulo marcado com sucesso. Retorna o status de conclusão da trilha e (opcionalmente) um novo badge.</response>
        /// <response code="400">Ocorreu um erro (ex: ID do módulo não existe). A mensagem de erro é retornada.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpPost("progress")]
        [ProducesResponseType(typeof(ProgressResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProgressResponseDto>> MarkProgress([FromBody] MarkModuleDto dto)
        {
            // Código de Produção: Pega o ID do usuário de dentro do token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var result = await _progressService.MarkModuleAsCompletedAsync(userId, dto.ModuleId);

                // O 'result' informa ao app se a trilha foi concluída 
                // e se um novo badge foi ganho, para o app exibir a animação.
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Captura erros do serviço (ex: "Módulo não encontrado.")
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Busca o painel (dashboard) completo do usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Este é o endpoint principal da tela inicial do app.
        /// Retorna o progresso (RF-010), dados para "continuar" (RF-007),
        /// badges ganhos (RF-012) e recomendações (RF-011).
        /// </remarks>
        /// <response code="200">Retorna o objeto DashboardDto completo.</response>
        /// <response code="400">Ocorreu um erro ao buscar o perfil (ex: usuário não encontrado no banco).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        [HttpGet("me")]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardDto>> GetMyProfile()
        {
            // Código de Produção: Pega o ID do usuário de dentro do token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                var dashboard = await _progressService.GetUserDashboardAsync(userId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                // Captura erros do serviço (ex: "Usuário não encontrado.")
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
