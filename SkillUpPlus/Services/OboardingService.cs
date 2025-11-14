using SkillUpPlus.Data;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace SkillUpPlus.Services
{
    public class OnboardingService : IOnboardingService
    {
        private readonly AppDbContext _context;

        public OnboardingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<InterestTagDto>> GetAllTagsAsync()
        {
            return await _context.InterestTags
                .Select(t => new InterestTagDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
        }

        public async Task SaveUserInterestsAsync(string userId, List<int> tagIds)
        {
            // 1. Limpar interesses antigos (estratégia de substituição completa)
            var existingInterests = await _context.UserInterests
                .Where(ui => ui.UserId == userId)
                .ToListAsync();

            _context.UserInterests.RemoveRange(existingInterests);

            // 2. Adicionar os novos
            // Verifica quais IDs passados realmente existem no banco para evitar erro de FK
            var validTags = await _context.InterestTags
                .Where(t => tagIds.Contains(t.Id))
                .Select(t => t.Id)
                .ToListAsync();

            foreach (var tagId in validTags)
            {
                _context.UserInterests.Add(new UserInterest
                {
                    UserId = userId,
                    InterestTagId = tagId
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}
