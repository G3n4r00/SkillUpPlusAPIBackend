using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillUpPlus.Models
{
    public class InterestTag
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore]
        public ICollection<UserInterest> UserInterests { get; set; } = new List<UserInterest>();
    }
}
