using TaskManagement.API.Models.Entities;
using Task = TaskManagement.API.Models.Entities.Task;

namespace TaskManagement.API.Repositories.Interfaces;

public interface ITaskAssignmentRepository:IGenericRepository<TaskAssignment>
{
    Task<IEnumerable<TaskAssignment>> GetActiveAssignmentsByTaskIdAsync(Guid taskId);
    Task<IEnumerable<TaskAssignment>> GetAssignmentsByUserIdAsync(Guid userId);
    Task<TaskAssignment?> GetActiveAssignmentAsync(Guid taskId, Guid userId);
    Task UnassignAsync(Guid taskId, Guid userId);
}