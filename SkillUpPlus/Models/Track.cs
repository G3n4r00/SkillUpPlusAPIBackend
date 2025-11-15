using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SkillUpPlus.Models
{
    public class Track
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Ex: IA, Soft Skills
        public int EstimatedTimeMinutes { get; set; }

        public ICollection<Module> Modules { get; set; } = new List<Module>();
    }

    public class Module
    {
        public int Id { get; set; }

        public int TrackId { get; set; }
        [JsonIgnore] // Evita ciclos na serialização
        public Track Track { get; set; } = null!;

        [Required]
        public string Title { get; set; } = string.Empty;

        // Conteúdo pode ser Markdown ou JSON
        public string Content { get; set; } = string.Empty;

        public string ModuleType { get; set; } = "text"; // text, quiz, video

        public int XpPoints { get; set; } = 10;
    }
}

