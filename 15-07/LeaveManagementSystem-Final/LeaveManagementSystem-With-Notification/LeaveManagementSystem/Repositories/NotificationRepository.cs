using LeaveManagementSystem.Contexts;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq; 
using System.Threading.Tasks;

namespace LeaveManagementSystem.Repositories
{
    public class NotificationRepository : Repository<Guid, Notification>, INotificationRepository
    {
        public NotificationRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }

        public override async Task<Notification> Get(Guid key)
        {
            return await _applicationDbContext.Notifications.FirstOrDefaultAsync(n => n.Id == key);
        }

        public override async Task<IEnumerable<Notification>> GetAll()
        {
            return await _applicationDbContext.Notifications.ToListAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _applicationDbContext.Notifications
                .Where(n => n.RecipientId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            await _applicationDbContext.SaveChangesAsync();
        }
    }
}
