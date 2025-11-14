using SkillUpPlus.DTOs;

namespace SkillUpPlus.Services
{
    public interface IOnboardingService
    {

            // Método usado pelo GET /api/onboarding/tags
            Task<IEnumerable<InterestTagDto>> GetAllTagsAsync();

            // Método usado pelo POST /api/onboarding
            Task SaveUserInterestsAsync(string userId, List<int> tagIds);
        
    }
}
