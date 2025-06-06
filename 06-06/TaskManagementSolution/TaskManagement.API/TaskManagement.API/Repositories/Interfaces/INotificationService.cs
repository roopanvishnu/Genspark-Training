namespace TaskManagement.API.Repositories.Interfaces;

public interface INotificationService
{
    Task NotifyTaskCreatedAsync(Guid taskId, string taskTitle, string createdBy);
    Task NotifyTaskUpdatedAsync(Guid taskId, string taskTitle, string updatedBy);
    Task NotifyTaskAssignedAsync(Guid taskId, string taskTitle, Guid assignedToUserId, string assignedBy);
    Task NotifyTaskStatusChangedAsync(Guid taskId, string taskTitle, string oldStatus, string newStatus, string updatedBy);
    Task NotifyFileUploadedAsync(Guid taskId, string fileName, string uploadedBy);
}