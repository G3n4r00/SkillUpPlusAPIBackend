using Microsoft.EntityFrameworkCore;
using SkillUpPlus.Data;
using SkillUpPlus.DTOs;

namespace SkillUpPlus.Services
{
    public class TrackService : ITrackService
    {
        private readonly AppDbContext _context;

        public TrackService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TrackSummaryDto>> GetAllTracksAsync(string? category)
        {
            // Inicia a query
            var query = _context.Tracks.AsQueryable();

            // Aplica filtro se a categoria for informada
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(t => t.Category == category);
            }

            // Projeção para DTO (Performance: seleciona apenas colunas necessárias)
            return await query.Select(t => new TrackSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Category = t.Category,
                EstimatedTimeMinutes = t.EstimatedTimeMinutes,
                ModuleCount = t.Modules.Count // Contagem otimizada pelo EF Core
            }).ToListAsync();
        }

        public async Task<TrackDetailDto?> GetTrackByIdAsync(int id)
        {
            // Busca a trilha INCLUINDO os módulos (Eager Loading)
            var track = await _context.Tracks
                .Include(t => t.Modules)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (track == null) return null;

            // Mapeamento manual (poderia usar AutoMapper, mas faremos manual por clareza)
            return new TrackDetailDto
            {
                Id = track.Id,
                Title = track.Title,
                Description = track.Description,
                Category = track.Category,
                EstimatedTimeMinutes = track.EstimatedTimeMinutes,
                ModuleCount = track.Modules.Count,
                Modules = track.Modules.Select(m => new ModuleDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Content = m.Content, // Aqui vai o texto/markdown
                    ModuleType = m.ModuleType
                }).ToList()
            };
        }
    }
}
