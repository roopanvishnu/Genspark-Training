using TaskManagement.API.Repositories.Interfaces;

namespace TaskManagement.API.Services.Implementations;

public class NotificationService : INotificationService
{
    public Task NotifyTaskCreatedAsync(Guid taskId, string taskTitle, string createdBy)
    {
        // Placeholder for real-time notifications
        return Task.CompletedTask;
    }

    public Task NotifyTaskUpdatedAsync(Guid taskId, string taskTitle, string updatedBy)
    {
        return Task.CompletedTask;
    }

    public Task NotifyTaskAssignedAsync(Guid taskId, string taskTitle, Guid assignedToUserId, string assignedBy)
    {
        return Task.CompletedTask;
    }

    public Task NotifyTaskStatusChangedAsync(Guid taskId, string taskTitle, string oldStatus, string newStatus, string updatedBy)
    {
        return Task.CompletedTask;
    }

    public Task NotifyFileUploadedAsync(Guid taskId, string fileName, string uploadedBy)
    {
        return Task.CompletedTask;
    }
}
