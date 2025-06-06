using TaskManagement.API.Models.Entities;
using TaskManagement.API.Models.Enums;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IUserRepository: IGenericRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
    Task<bool> EmailExistAsync(string email);
    Task<IEnumerable<User>>GetActiveUsersAsync();
    Task<IEnumerable<User>>GetUserByRoleAsync(UserRole role);
}