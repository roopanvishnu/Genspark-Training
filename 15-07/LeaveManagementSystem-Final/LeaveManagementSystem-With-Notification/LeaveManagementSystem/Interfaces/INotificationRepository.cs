using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Services;
using LeaveManagementSystem.Models;
namespace LeaveManagementSystem.Interfaces {
    public interface INotificationRepository : IRepository<Guid, Notification>
    {
        Task MarkAllAsReadAsync(Guid userId);
    }

}