using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;
using SkillUpPlus.Services;

namespace SkillUpPlus.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        public AuthController(UserManager<User> userManager, ITokenService tokenService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserTokenDto>> Register(RegisterDto dto)
        {
            // Verifica se o e-mail já está em uso
            if (await _userManager.Users.AnyAsync(x => x.Email == dto.Email))
            {
                return BadRequest("Este e-mail já está em uso.");
            }

            // Cria o objeto do usuário
            var user = new User
            {
                Email = dto.Email,
                UserName = dto.Email, // Identity usa UserName para login
                Name = dto.Name,
                CreatedAt = DateTime.UtcNow
            };

            // Tenta criar o usuário no banco (o Identity cuida do HASH da senha)
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            // Se deu certo, responde com os dados do usuário + um Token
            return new UserTokenDto
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.Name,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserTokenDto>> Login(LoginDto dto)
        {
            // Encontra o usuário pelo e-mail
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            // Verifica se a senha está correta (o Identity compara os hashes)
            var result = await _userManager.CheckPasswordAsync(user, dto.Password);

            if (!result)
            {
                return Unauthorized("E-mail ou senha inválidos.");
            }

            // Sucesso! Retorna os dados do usuário + um Token
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
