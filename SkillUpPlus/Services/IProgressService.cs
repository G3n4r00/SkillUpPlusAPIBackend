using SkillUpPlus.DTOs;

namespace SkillUpPlus.Services
{
    public interface IProgressService
    {
        // --- V1 ---
        Task<ProgressResponseDto> MarkModuleAsCompletedAsync(string userId, int moduleId);
        Task<DashboardDto> GetUserDashboardAsync(string userId);

        // --- V2 ---

        Task<ProgressResponseV2Dto> MarkModuleAsCompletedV2Async(string userId, int moduleId);


        Task<DashboardV2Dto> GetUserDashboardV2Async(string userId);


        Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync();
    }
}
