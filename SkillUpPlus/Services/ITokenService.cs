using SkillUpPlus.Models;

namespace SkillUpPlus.Services
{
    // O "contrato" que define o que o serviço de token deve fazer
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
