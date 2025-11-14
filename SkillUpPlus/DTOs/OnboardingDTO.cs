using System.ComponentModel.DataAnnotations;

namespace SkillUpPlus.DTOs
{
    // Para listar as tags na tela (GET)
    public class InterestTagDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // Para o usuário enviar suas escolhas (POST)
    public class OnboardingRequestDto
    {
        [Required]
        public List<int> InterestIds { get; set; } = new List<int>();
    }
}
