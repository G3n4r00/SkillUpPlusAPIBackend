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

            // Validar se o módulo existe e pegar dados da Trilha (Track)
            var module = await _context.Modules
                .Include(m => m.Track) // de qual trilha é
                .FirstOrDefaultAsync(m => m.Id == moduleId);

            if (module == null) throw new Exception("Módulo não encontrado.");

            // Registrar o progresso (Idempotente)
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


            // Verificar se a trilha foi completada
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

        public async Task<DashboardDto> GetUserDashboardAsync(string userId)
        {
            //Carregar User COM Interesses(Include essencial!)
            var user = await _context.Users
            .Include(u => u.Badges).ThenInclude(ub => ub.Badge)
            .Include(u => u.Interests).ThenInclude(ui => ui.InterestTag) // <--- NOVO INCLUDE
            .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) throw new Exception("Usuário não encontrado.");

            // Recuperar todo o histórico de progresso desse usuário
            // Também os dados dos Módulos e Trilhas para evitar múltiplas queries
            var userProgress = await _context.UserProgresses
                .Include(up => up.Module)
                .ThenInclude(m => m.Track)
                .Where(up => up.UserId == userId)
                .ToListAsync();

            // Recuperar TODAS as trilhas e seus módulos para saber o total de cada uma
            var allTracks = await _context.Tracks
                .Include(t => t.Modules)
                .ToListAsync();

            // Montar o DTO
            var dashboard = new DashboardDto
            {
                UserId = user.Id,
                UserName = user.Name,
                Badges = user.Badges.Select(b => new BadgeDto
                {
                    Id = b.BadgeId,
                    Name = b.Badge.Name,
                    IconUrl = b.Badge.IconUrl
                }).ToList()
            };

            // Processar cada trilha do sistema para ver o status do usuário nela
            foreach (var track in allTracks)
            {
                var trackModulesIds = track.Modules.Select(m => m.Id).ToHashSet();
                var completedModulesCount = userProgress.Count(up => trackModulesIds.Contains(up.ModuleId));
                var totalModules = track.Modules.Count;

                if (completedModulesCount == 0) continue; // Não começou, não mostra no dashboard

                var trackDto = new TrackProgressDto
                {
                    TrackId = track.Id,
                    Title = track.Title,
                    Category = track.Category,
                    TotalModules = totalModules,
                    CompletedModules = completedModulesCount
                };

                if (completedModulesCount >= totalModules)
                {
                    // Trilha Concluída
                    trackDto.NextModuleId = null; // Não tem próximo
                    dashboard.CompletedTracks.Add(trackDto);
                }
                else
                {
                    // Trilha Em Andamento 
                    // Descobre qual é o primeiro módulo que AINDA NÃO foi feito
                    var completedIds = userProgress
                        .Where(up => trackModulesIds.Contains(up.ModuleId))
                        .Select(up => up.ModuleId)
                        .ToHashSet();

                    var nextModule = track.Modules
                        .OrderBy(m => m.Id) // Assume ordem por ID; idealmente teria um campo 'Order'
                        .FirstOrDefault(m => !completedIds.Contains(m.Id));

                    trackDto.NextModuleId = nextModule?.Id;
                    dashboard.InProgressTracks.Add(trackDto);
                }


            }

            var userInterestNames = user.Interests
            .Select(i => i.InterestTag.Name.ToLower())
            .ToList();

            // Filtra trilhas que:
            // 1. O usuário AINDA NÃO completou (ou não começou)
            // 2. A categoria da trilha bate com algum interesse
            var alreadyCompletedTrackIds = dashboard.CompletedTracks.Select(t => t.TrackId).ToHashSet();
            var inProgressTrackIds = dashboard.InProgressTracks.Select(t => t.TrackId).ToHashSet();

            foreach (var track in allTracks)
            {
                // Ignora se já completou ou está fazendo
                if (alreadyCompletedTrackIds.Contains(track.Id) || inProgressTrackIds.Contains(track.Id))
                    continue;

                // Verifica match de categoria
                // Ex: Categoria "Inteligência Artificial" dá match com tag "Inteligência Artificial"
                bool matchesInterest = userInterestNames.Any(tag =>
                    track.Category.ToLower().Contains(tag) ||
                    tag.Contains(track.Category.ToLower()));

                if (matchesInterest || userInterestNames.Count == 0)
                {
                    // Cria o DTO (Reutilizando TrackProgressDto com 0% de progresso)
                    dashboard.RecommendedTracks.Add(new TrackProgressDto
                    {
                        TrackId = track.Id,
                        Title = track.Title,
                        Category = track.Category,
                        TotalModules = track.Modules.Count,
                        CompletedModules = 0,
                        NextModuleId = track.Modules.FirstOrDefault()?.Id // Começa do inicio
                    });
                }
            }

            return dashboard;
        }

        //V2
        public async Task<ProgressResponseV2Dto> MarkModuleAsCompletedV2Async(string userId, int moduleId)
        {
            var v1Response = await MarkModuleAsCompletedAsync(userId, moduleId);

            // Busca o XP do módulo que acabamos de completar
            var module = await _context.Modules.FindAsync(moduleId);
            var xpGained = module?.XpPoints ?? 0; // default 0 se não achar

            return new ProgressResponseV2Dto
            {
                IsTrackCompleted = v1Response.IsTrackCompleted,
                NewBadgeEarned = v1Response.NewBadgeEarned,
                XpGained = xpGained
            };
        }

        public async Task<DashboardV2Dto> GetUserDashboardV2Async(string userId)
        {
            var v1Dashboard = await GetUserDashboardAsync(userId);

            var totalXp = await _context.UserProgresses
                .Where(up => up.UserId == userId)
                .Include(up => up.Module) 
                .SumAsync(up => up.Module.XpPoints); 

            return new DashboardV2Dto
            {
                UserId = v1Dashboard.UserId,
                UserName = v1Dashboard.UserName,
                Badges = v1Dashboard.Badges,
                InProgressTracks = v1Dashboard.InProgressTracks,
                CompletedTracks = v1Dashboard.CompletedTracks,
                RecommendedTracks = v1Dashboard.RecommendedTracks,
                TotalXp = totalXp 
            };
        }

        public async Task<IEnumerable<LeaderboardEntryDto>> GetLeaderboardAsync()
        {
            var leaderboardData = await _context.Users
                .Select(u => new
                {
                    UserId = u.Id,
                    UserName = u.Name,
                    TotalXp = u.ProgressHistory.Sum(p => p.Module.XpPoints),
                    BadgesCount = u.Badges.Count
                })
                .OrderByDescending(x => x.TotalXp) // Ordena por XP
                .ThenBy(x => x.BadgesCount)      // Desempate por badges
                .Take(10)                       // Top 10
                .ToListAsync();

            // Mapeia para o DTO final com o Rank
            return leaderboardData.Select((entry, index) => new LeaderboardEntryDto
            {
                Rank = index + 1,
                UserId = entry.UserId,
                UserName = entry.UserName,
                TotalXp = entry.TotalXp,
                BadgesCount = entry.BadgesCount
            });

        }
    }
}