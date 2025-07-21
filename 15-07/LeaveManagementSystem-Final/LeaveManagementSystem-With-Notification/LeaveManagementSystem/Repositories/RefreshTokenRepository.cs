// Repositories/RefreshTokenRepository.cs
using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Repositories
{
    public class RefreshTokenRepository : Repository<Guid, RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<RefreshToken> Get(Guid key)
        {
            return await _applicationDbContext.RefreshTokens
                .Include(rt => rt.User) // optional, if you need user info
                .FirstOrDefaultAsync(rt => rt.Id == key)
                   ?? throw new Exception("Refresh token not found");
        }

        public override async Task<IEnumerable<RefreshToken>> GetAll()
        {
            return await _applicationDbContext.RefreshTokens
                .Include(rt => rt.User)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetByToken(string token)
        {
            return await _applicationDbContext.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == token && !rt.IsRevoked);
        }

        public async Task<IEnumerable<RefreshToken>> GetTokensByUserId(Guid userId)
        {
            return await _applicationDbContext.RefreshTokens
                .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                .ToListAsync();
        }
    }
}
