using TaskManagement.API.Models.Entities;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IFileAttachmentRepository:IGenericRepository<FileAttachment>
{
    Task<IEnumerable<FileAttachment>> GetFilesByTaskIdAsync(Guid taskId);
    Task<FileAttachment?> GetByFileNameAsync(string fileName);
    Task<long> GetTotalSizeAsync(Guid taskId);
}