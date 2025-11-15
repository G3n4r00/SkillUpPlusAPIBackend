using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SkillUpPlus.Models
{
    // IdentityUser inclui Id (string), Email, UserName, PhoneNumber, etc.
    public class User : IdentityUser
    {

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserInterest> Interests { get; set; } = new List<UserInterest>();
        public ICollection<UserProgress> ProgressHistory { get; set; } = new List<UserProgress>();
        public ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();
    }
}

