using System.ComponentModel.DataAnnotations;

namespace SkillUpPlus.DTOs
{
    public class UserSyncDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // O ID será extraido do Token de segurança para mais segurança.
    }
}
