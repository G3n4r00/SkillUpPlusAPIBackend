using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;
using SkillUpPlus.Services;
using System.Security.Claims;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Rota: api/users
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // POST: api/users/sync
        [HttpPost("sync")]
        public async Task<IActionResult> SyncUser([FromBody] UserSyncDto dto)
        {
            // 1. Extrair o UID do Firebase de dentro do Token (Segurança Máxima)
            // O Firebase geralmente manda o UID na claim "user_id" ou NameIdentifier
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Token inválido: ID do usuário não encontrado.");
            }

            // 2. Chamar o serviço para sincronizar
            var user = await _userService.SyncUserAsync(userId, dto);

            // 3. Retornar 200 OK com os dados do usuário
            return Ok(user);
        }
    }
}
