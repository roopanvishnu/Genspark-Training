using LeaveManagementSystem.Models;
namespace LeaveManagementSystem.Interfaces {
    public interface INotificationService
    {
        Task CreateAsync(Guid recipientId, string message, Guid? reviewedById = null);
        Task CreateForMultipleAsync(List<Guid> recipientIds, string message, Guid? reviewedById = null);
        Task<List<Notification>> GetUnreadAsync(Guid recipientId);
        Task<List<Notification>> GetAllAsync(Guid recipientId);
        Task MarkAsReadAsync(Guid notificationId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}
