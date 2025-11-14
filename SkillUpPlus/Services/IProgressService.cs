using SkillUpPlus.DTOs;

namespace SkillUpPlus.Services
{
    public interface IProgressService
    {
        // Retorna um DTO informando se completou a trilha e se ganhou badge
        Task<ProgressResponseDto> MarkModuleAsCompletedAsync(string userId, int moduleId);
    }
}
