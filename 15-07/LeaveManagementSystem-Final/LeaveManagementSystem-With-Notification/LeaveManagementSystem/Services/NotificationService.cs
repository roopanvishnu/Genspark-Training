// Services/NotificationService.cs
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.AspNetCore.SignalR;
using LeaveManagementSystem.Hubs;

namespace LeaveManagementSystem.Services {
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hub;
        private readonly INotificationRepository _notificationRepo;


        public NotificationService(INotificationRepository notificationRepo, IHubContext<NotificationHub> hub)
        {
            _notificationRepo = notificationRepo;
            _hub = hub;
        }


        public async Task CreateAsync(Guid recipientId, string message, Guid? reviewedById = null)
        {
            var notification = new Notification
            {
                Id = Guid.NewGuid(),
                RecipientId = recipientId,
                Message = message,
                ReviewedById = reviewedById,
                CreatedAt = DateTime.UtcNow
            };

            await _notificationRepo.Add(notification);

            await _hub.Clients.Group($"USER_{recipientId}")
                .SendAsync("ReceiveNotification", message);
        }

        public async Task CreateForMultipleAsync(List<Guid> recipientIds, string message, Guid? reviewedById = null)
        {
            foreach (var id in recipientIds)
            {
                await CreateAsync(id, message, reviewedById);
            }
        }

        public async Task<List<Notification>> GetUnreadAsync(Guid recipientId)
        {
            var all = await _notificationRepo.GetAll();
            return all
                .Where(n => n.RecipientId == recipientId && !n.IsRead && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task<List<Notification>> GetAllAsync(Guid recipientId)
        {
            var all = await _notificationRepo.GetAll();
            return all
                .Where(n => n.RecipientId == recipientId && !n.IsDeleted)
                .OrderByDescending(n => n.CreatedAt)
                .ToList();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var n = await _notificationRepo.Get(notificationId);
            if (n != null)
            {
                n.IsRead = true;
                n.ReadAt = DateTime.UtcNow;
                await _notificationRepo.Update(notificationId, n);
            }
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            await _notificationRepo.MarkAllAsReadAsync(userId);
        }

    }

}