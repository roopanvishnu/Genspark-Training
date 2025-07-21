using LeaveManagementSystem.Models;

namespace LeaveManagementSystem.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<Guid, RefreshToken>
    {
        Task<RefreshToken?> GetByToken(string token);
        Task<IEnumerable<RefreshToken>> GetTokensByUserId(Guid userId);
    }
}
