using Microsoft.IdentityModel.Tokens;
using SkillUpPlus.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SkillUpPlus.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        }

        public string CreateToken(User user)
        {
            // Define as informações que irão DENTRO do token
            var claims = new List<Claim>
            {
                // Este é o 'subject' (ID do usuário). quem é o usuário em CADA requisição [Authorize].
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.Name)
            };

            // Define as credenciais de assinatura
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            // Monta o "descritor" do token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"]
            };

            // Cria e escreve o token como uma string
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
