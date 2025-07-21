// Controllers/NotificationsController.cs
using Microsoft.AspNetCore.Mvc;
using LeaveManagementSystem.Interfaces;


namespace LeaveManagementSystem.Controllers {
    [ApiController]
    [Route("api/v1/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("unread/{userId}")]
        public async Task<IActionResult> GetUnread(Guid userId)
        {
            var list = await _notificationService.GetUnreadAsync(userId);
            return Ok(list);
        }

        [HttpGet("all/{userId}")]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var list = await _notificationService.GetAllAsync(userId);
            return Ok(list);
        }

        [HttpPost("mark-as-read/{id}")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return NoContent();
        }

        [HttpPost("mark-all-as-read/{userId}")]
        public async Task<IActionResult> MarkAllAsRead(Guid userId)
        {
            await _notificationService.MarkAllAsReadAsync(userId);
            return NoContent();
        }

    }
}
