namespace SkillUpPlus.Models
{
    public class UserProgress
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;

        public int ModuleId { get; set; }
        public Module Module { get; set; } = null!;

        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }
}
