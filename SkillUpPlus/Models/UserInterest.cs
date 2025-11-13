using System.Text.Json.Serialization;

namespace SkillUpPlus.Models
{
        public class UserInterest
        {
            // Chave composta (UserId + InterestTagId) será definida no DbContext
            public string UserId { get; set; } = string.Empty;
            [JsonIgnore]
            public User User { get; set; } = null!;

            public int InterestTagId { get; set; }
            public InterestTag InterestTag { get; set; } = null!;
        }
    
}
