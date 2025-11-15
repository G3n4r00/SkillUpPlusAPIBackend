namespace SkillUpPlus.DTOs
{

    public class ProgressResponseV2Dto : ProgressResponseDto
    {
        public int XpGained { get; set; }
    }

    public class DashboardV2Dto : DashboardDto
    {
        public int TotalXp { get; set; }
    }

    public class LeaderboardEntryDto
    {
        public int Rank { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int TotalXp { get; set; }
        public int BadgesCount { get; set; }
    }
}
