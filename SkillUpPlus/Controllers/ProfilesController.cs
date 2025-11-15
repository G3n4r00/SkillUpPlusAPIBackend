using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;
using System.Security.Claims;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [ApiVersion("1.0")] // Este controller responde à v1...
    [ApiVersion("2.0")] // ...e também à v2
    [Route("api/v{version:apiVersion}/profile")]
    [Authorize]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProfileController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        #region === Endpoints v1.0 ===

        /// <summary>
        /// (v1.0) Marca um módulo como concluído (sem XP).
        /// </summary>
        [HttpPost("progress")]
        [MapToApiVersion("1.0")] // <-- Trava este método na v1
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
        /// (v1.0) Busca o painel (dashboard) do usuário (sem XP).
        /// </summary>
        [HttpGet("me")]
        [MapToApiVersion("1.0")] // <-- Trava este método na v1
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

        #endregion

        #region === Endpoints v2.0 (Novos) ===

        /// <summary>
        /// (v2.0) Marca um módulo como concluído e retorna o XP ganho.
        /// </summary>
        [HttpPost("progress")]
        [MapToApiVersion("2.0")] // <-- Trava este método na v2
        [ProducesResponseType(typeof(ProgressResponseV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ProgressResponseV2Dto>> MarkProgressV2([FromBody] MarkModuleDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                // Chama o novo método do serviço
                var result = await _progressService.MarkModuleAsCompletedV2Async(userId, dto.ModuleId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// (v2.0) Busca o painel (dashboard) do usuário, incluindo XP total.
        /// </summary>
        [HttpGet("me")]
        [MapToApiVersion("2.0")] // <-- Trava este método na v2
        [ProducesResponseType(typeof(DashboardV2Dto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<DashboardV2Dto>> GetMyProfileV2()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            try
            {
                // Chama o novo método do serviço
                var dashboard = await _progressService.GetUserDashboardV2Async(userId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        #endregion
    }
}
