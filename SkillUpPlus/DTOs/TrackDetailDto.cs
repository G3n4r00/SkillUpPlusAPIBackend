namespace SkillUpPlus.DTOs
{
    public class TrackDetailDto : TrackSummaryDto
    {
        public List<ModuleDto> Modules { get; set; } = new List<ModuleDto>();
    }

    public class ModuleDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ModuleType { get; set; } = string.Empty;
    }
}
