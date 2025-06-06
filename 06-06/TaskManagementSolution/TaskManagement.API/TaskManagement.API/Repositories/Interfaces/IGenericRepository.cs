using System.Linq.Expressions;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
    Task<int> CountAsync(Expression<Func<T, bool>>? expression = null);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> expression);
}