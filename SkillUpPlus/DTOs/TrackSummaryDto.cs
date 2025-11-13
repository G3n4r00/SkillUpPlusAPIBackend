namespace SkillUpPlus.DTOs
{
    public class TrackSummaryDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int EstimatedTimeMinutes { get; set; }
        public int ModuleCount { get; set; }
    }
}
