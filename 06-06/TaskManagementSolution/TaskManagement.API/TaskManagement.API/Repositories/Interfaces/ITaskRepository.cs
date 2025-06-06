using TaskManagement.API.DTOs.Pagination;
using TaskManagement.API.DTOs.Request.Task;

namespace TaskManagement.API.Repositories.Interfaces;

public interface ITaskRepository :IGenericRepository<Models.Entities.Task>
{
    Task<PagedResult<Models.Entities.Task>> GetFilteredTasksAsync(TaskFilterRequestDto filter, PaginationParameters pagination);
    Task<IEnumerable<Models.Entities.Task>> GetTasksByUserIdAsync(Guid userId);
    Task<IEnumerable<Models.Entities.Task>> GetTasksByStatusAsync(Models.Enums.TaskStatus status);
    Task<Models.Entities.Task?> GetTaskWithDetailsAsync(Guid taskId);
    Task<IEnumerable<Models.Entities.Task>> GetOverdueTasksAsync();
}