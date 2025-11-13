using SkillUpPlus.DTOs;
using SkillUpPlus.Models;

namespace SkillUpPlus.Services
{
    public interface IUserService
    {
        Task<User> SyncUserAsync(string firebaseUserId, UserSyncDto dto);
    }
}
