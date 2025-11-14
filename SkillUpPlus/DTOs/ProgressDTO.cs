namespace SkillUpPlus.DTOs
{
    // O que o App envia
    public class MarkModuleDto
    {
        public int ModuleId { get; set; }
    }

    // O que a API responde
    public class ProgressResponseDto
    {
        public bool IsTrackCompleted { get; set; }
        public BadgeDto? NewBadgeEarned { get; set; } // Null se não ganhou badge agora
    }

    // DTO simples para o Badge
    public class BadgeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string IconUrl { get; set; } = string.Empty;
    }
}
