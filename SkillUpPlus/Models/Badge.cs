using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillUpPlus.Models
{
    public class Badge
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; // Ex: "Mestre da Comunicação"

        public string IconUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
    }
}
