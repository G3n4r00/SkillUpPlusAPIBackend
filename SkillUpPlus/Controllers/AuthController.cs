using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;
using SkillUpPlus.Services;
using Asp.Versioning;

namespace SkillUpPlus.Controllers
{
    /// <summary>
    /// Gerencia o registro e login de usuários (Autenticação v2).
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra um novo usuário na plataforma.
        /// </summary>
        /// <param name="dto">Dados de registro (e-mail, nome, senha).</param>
        /// <response code="200">Retorna os dados do usuário e um JWT válido.</response>
        /// <response code="400">Dados inválidos (ex: e-mail já em uso ou senha fraca).</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserTokenDto>> Register(RegisterDto dto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == dto.Email))
            {
                return BadRequest("Este e-mail já está em uso.");
            }

            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                // Retorna os erros do Identity (ex: senha muito curta)
                return BadRequest(result.Errors);
            }

            return new UserTokenDto
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }

        /// <summary>
        /// Autentica um usuário existente e retorna um JWT.
        /// </summary>
        /// <param name="dto">Dados de login (e-mail, senha).</param>
        /// <response code="200">Retorna os dados do usuário e um JWT válido.</response>
        /// <response code="401">E-mail ou senha inválidos.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<UserTokenDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!result)
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            return new UserTokenDto
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }
    }
}
