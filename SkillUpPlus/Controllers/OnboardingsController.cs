using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;
using System.Security.Claims;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        // GET: api/onboarding/tags
        // Para preencher a tela de seleção
        [HttpGet("tags")]
        public async Task<ActionResult<IEnumerable<InterestTagDto>>> GetTags()
        {
            var tags = await _onboardingService.GetAllTagsAsync();
            return Ok(tags);
        }

        // POST: api/onboarding
        // Salvar as preferências
        [HttpPost]
        public async Task<IActionResult> SavePreferences([FromBody] OnboardingRequestDto dto)
        {
            // --- MODO DE TESTE ---
            //var userId = "user_teste_id";
            // --------------------

            // PRODUÇÃO:
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            

            if (dto.InterestIds == null || !dto.InterestIds.Any())
            {
                return BadRequest("Selecione ao menos um interesse.");
            }

            await _onboardingService.SaveUserInterestsAsync(userId, dto.InterestIds);
            return Ok(new { message = "Preferências salvas com sucesso!" });
        }
    }
}
