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
    /// Gerencia o registro e login de usuários.
    /// </summary>
    /// <remarks>
    /// Este é o "cartão de visitas" da API. Não requer autenticação,
    /// endpoint é usado para OBTER o token de autenticação.
    /// </remarks>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Inicializa uma nova instância do AuthController.
        /// </summary>
        /// <param name="userManager">Serviço do Identity para gerenciar usuários (criar, checar senha).</param>
        /// <param name="tokenService">Serviço customizado para gerar tokens JWT.</param>
        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Registra um novo usuário na plataforma.
        /// </summary>
        /// <remarks>
        /// Cria a entidade do usuário no banco de dados, aplica hash na senha
        /// e, se for bem-sucedido, retorna um JWT válido para o novo usuário.
        /// </remarks>
        /// <param name="dto">Dados de registro (e-mail, nome, senha).</param>
        /// <response code="200">Retorna os dados do usuário e um JWT válido.</response>
        /// <response code="400">Dados inválidos (ex: "Este e-mail já está em uso.").</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(UserTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]

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
                return BadRequest(result.Errors);
            }

            // Se o registro foi OK, já loga o usuário e retorna o token
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
        /// <remarks>
        /// Verifica as credenciais (e-mail e senha) contra o banco de dados.
        /// Se forem válidas, gera um novo token JWT para o usuário.
        /// </remarks>
        /// <param name="dto">Dados de login (e-mail, senha).</param>
        /// <response code="200">Retorna os dados do usuário e um JWT válido.</response>
        /// <response code="401">E-mail ou senha inválidos.</response>
        /// <response code="500">Erro interno do servidor (ex: falha no banco de dados).</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(UserTokenDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(object), StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<UserTokenDto>> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
            }

            var result = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!result)
            {
                return Unauthorized(new { message = "E-mail ou senha inválidos." });
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

