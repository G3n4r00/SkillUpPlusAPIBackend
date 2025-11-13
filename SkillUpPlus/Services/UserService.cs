using Microsoft.EntityFrameworkCore;
using SkillUpPlus.Data;
using SkillUpPlus.DTOs;
using SkillUpPlus.Models;

namespace SkillUpPlus.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> SyncUserAsync(string firebaseUserId, UserSyncDto dto)
        {
            // Busca se o usuário já existe pelo ID do Firebase
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == firebaseUserId);

            if (user == null)
            {
                // Se não existe, CRIA (Fluxo de Primeiro Acesso)
                user = new User
                {
                    Id = firebaseUserId,
                    Email = dto.Email,
                    Name = dto.Name,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
            }
            else
            {
                // Se existe, ATUALIZA (caso ele tenha mudado o nome no Google/App)
                user.Name = dto.Name;
                user.Email = dto.Email; // O email pode ter mudado raramente, mas garantimos
            }

            await _context.SaveChangesAsync();
            return user;
        }
    }
}
