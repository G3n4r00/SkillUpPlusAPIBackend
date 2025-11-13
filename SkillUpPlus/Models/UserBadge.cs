using System.Text.Json.Serialization;

namespace SkillUpPlus.Models
{
    public class UserBadge
    {
        public int Id { get; set; } 

        public string UserId { get; set; } = string.Empty;
        [JsonIgnore]
        public User User { get; set; } = null!;

        public int BadgeId { get; set; }
        public Badge Badge { get; set; } = null!;

        public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
    }
}
