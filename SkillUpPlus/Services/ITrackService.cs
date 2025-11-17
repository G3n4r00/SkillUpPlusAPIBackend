using SkillUpPlus.DTOs;

namespace SkillUpPlus.Services
{
    public interface ITrackService
    {
        // Retorna lista resumida, opcionalmente filtrada por categoria
        Task<IEnumerable<TrackSummaryDto>> GetAllTracksAsync(string? category);

        // Retorna detalhes completos com módulos
        Task<TrackDetailDto> GetTrackByIdAsync(int id);
    }
}
