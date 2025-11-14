using SkillUpPlus.Data;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;
using Microsoft.EntityFrameworkCore;

namespace SkillUpPlus.Services
{
    public class ProgressService : IProgressService
    {
        private readonly AppDbContext _context;

        public ProgressService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ProgressResponseDto> MarkModuleAsCompletedAsync(string userId, int moduleId)
        {
            var response = new ProgressResponseDto();

            // 1. Validar se o módulo existe e pegar dados da Trilha (Track)
            var module = await _context.Modules
                .Include(m => m.Track) // de qual trilha é
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            if (module == null) throw new Exception("Módulo não encontrado.");

            // 2. Registrar o progresso (Idempotente: se já existe, ignora)
            var existingProgress = await _context.UserProgresses
                .FirstOrDefaultAsync(up => up.UserId == userId && up.ModuleId == moduleId);

            if (existingProgress == null)
            {
                _context.UserProgresses.Add(new UserProgress
                {
                    UserId = userId,
                    ModuleId = moduleId,
                    CompletedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();
            }


            // 3. Verificar se a trilha foi completada
            // Conta quantos módulos tem a trilha
            var totalModules = await _context.Modules.CountAsync(m => m.TrackId == module.TrackId);

            // Conta quantos módulos dessa trilha o usuário já fez
            var completedModules = await _context.UserProgresses
                .CountAsync(up => up.UserId == userId && up.Module.TrackId == module.TrackId);

            if (completedModules >= totalModules)
            {
                response.IsTrackCompleted = true;
                response.NewBadgeEarned = await AssignBadgeAsync(userId, module.Track.Category);
            }

            return response;
        }

        private async Task<BadgeDto?> AssignBadgeAsync(string userId, string category)
        {
            // Define qual badge dar baseado na categoria
            string badgeNameTarget = category switch
            {
                "Soft Skills" => "Mestre da Comunicação",
                "Inteligência Artificial" => "Futurista Tech",
                _ => "Pioneiro"
            };

            // Busca o badge no banco
            var badge = await _context.Badges.FirstOrDefaultAsync(b => b.Name == badgeNameTarget);
            if (badge == null) return null; // Badge não cadastrado no sistema

            // Verifica se o usuário JÁ tem esse badge (para não dar duplicado)
            var userHasBadge = await _context.UserBadges
                .AnyAsync(ub => ub.UserId == userId && ub.BadgeId == badge.Id);

            if (!userHasBadge)
            {
                // Concede o Badge
                _context.UserBadges.Add(new UserBadge
                {
                    UserId = userId,
                    BadgeId = badge.Id,
                    EarnedAt = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                // Retorna o DTO para o frontend exibir o modal de conquista
                return new BadgeDto
                {
                    Id = badge.Id,
                    Name = badge.Name,
                    IconUrl = badge.IconUrl
                };
            }

            return null; // Já tinha o badge, não é novidade
        }
    }
}
