using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
        // LEMBRAR DE TIRAR COMENTARIO
    public class ProfileController : ControllerBase
    {
        private readonly IProgressService _progressService;

        public ProfileController(IProgressService progressService)
        {
            _progressService = progressService;
        }

        // POST: api/profile/progress
        // Marcar módulo como concluído
        [HttpPost("progress")]
        public async Task<ActionResult<ProgressResponseDto>> MarkProgress([FromBody] MarkModuleDto dto)
        {
            // Simulação de User ID para testes sem Token (Remova isso em produção!)
            // Em produção use: var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // --- MODO DE TESTE: Pega o primeiro usuário do banco ou usa um ID fixo ---
            var userId = "user_teste_id"; // Mesmo ID que usaremos para testar
            // -----------------------------------------------------------------------

            /* CÓDIGO REAL (Descomente depois):
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            */

            try
            {
                var result = await _progressService.MarkModuleAsCompletedAsync(userId, dto.ModuleId);

                // Se ganhou badge, o frontend vai saber pelo result.NewBadgeEarned
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
