namespace SkillUpPlus.DTOs
{
    public class DashboardDto
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        // Gamificação
        public List<BadgeDto> Badges { get; set; } = new List<BadgeDto>();

        // Status das Trilhas
        public List<TrackProgressDto> InProgressTracks { get; set; } = new List<TrackProgressDto>();
        public List<TrackProgressDto> CompletedTracks { get; set; } = new List<TrackProgressDto>();
        public List<TrackProgressDto> RecommendedTracks { get; set; } = new List<TrackProgressDto>();
    }

    public class TrackProgressDto
    {
        public int TrackId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        // Dados vitais para a Barra de Progresso
        public int TotalModules { get; set; }
        public int CompletedModules { get; set; }
        public int PercentageComplete => TotalModules == 0 ? 0 : (int)((double)CompletedModules / TotalModules * 100);

        // Para o botão "Continuar de onde parou"
        public int? NextModuleId { get; set; }
    }
}
