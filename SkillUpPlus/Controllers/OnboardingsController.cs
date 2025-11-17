using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Services;
using System.Security.Claims;
using Asp.Versioning;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia o processo de Onboarding do usuário, incluindo a seleção de interesses.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize] 
    [Produces("application/json")]
    public class OnboardingController : ControllerBase
    {
        private readonly IOnboardingService _onboardingService;

        /// <summary>
        /// Inicializa uma nova instância do OnboardingController.
        /// </summary>
        /// <param name="onboardingService">O serviço injetado que contém a lógica de onboarding.</param>
        public OnboardingController(IOnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        /// <summary>
        /// Busca a lista completa de tags de interesse disponíveis na plataforma.
        /// </summary>
        /// <remarks>
        /// O usuário deve estar autenticado. O app usa esta lista para exibir as opções na tela de onboarding.
        /// </remarks>
        /// <response code="200">Retorna a lista de tags de interesse.</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
        [HttpGet("tags")]
        [ProducesResponseType(typeof(IEnumerable<InterestTagDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<InterestTagDto>>> GetTags()
        {
            var tags = await _onboardingService.GetAllTagsAsync();
            return Ok(tags);
        }

        /// <summary>
        /// Salva as preferências de interesse (tags) para o usuário autenticado.
        /// </summary>
        /// <remarks>
        /// Este endpoint apaga as preferências antigas e salva a nova lista.
        /// </remarks>
        /// <param name="dto">Um objeto JSON contendo a lista de IDs das tags selecionadas.</param>
        /// <response code="200">Preferências salvas com sucesso.</response>
        /// <response code="400">Nenhum ID de interesse foi fornecido (lista vazia).</response>
        /// <response code="401">Usuário não autenticado (token inválido ou ausente).</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
        [HttpPost]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> SavePreferences([FromBody] OnboardingRequestDto dto)
        {
            // Busca o ID do usuário a partir do Token JWT
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            if (dto.InterestIds == null || !dto.InterestIds.Any())
            {
                return BadRequest("Selecione ao menos um interesse.");
            }

            await _onboardingService.SaveUserInterestsAsync(userId, dto.InterestIds);
            return Ok(new { message = "Preferências salvas com sucesso!" });
        }
    }
}
