using System.ComponentModel.DataAnnotations;

namespace SkillUpPlus.Models
{
    public class User
    {

        [Key]
        public string Id { get; set; } = string.Empty; // UID do Firebase

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamentos
        public ICollection<UserInterest> Interests { get; set; } = new List<UserInterest>();
        public ICollection<UserProgress> ProgressHistory { get; set; } = new List<UserProgress>();
        public ICollection<UserBadge> Badges { get; set; } = new List<UserBadge>();

    }
}

