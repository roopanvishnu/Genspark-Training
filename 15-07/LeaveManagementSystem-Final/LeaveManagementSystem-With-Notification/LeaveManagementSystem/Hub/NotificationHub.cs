using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Security.Claims;


namespace LeaveManagementSystem.Hubs
{
public class NotificationHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        string email = Context.User?.FindFirst(ClaimTypes.Email)?.Value;
        Console.WriteLine($"✅ Connected: {Context.ConnectionId}, Email: {email}");

        if (!string.IsNullOrEmpty(email))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"USER_{email}");
            Console.WriteLine($"👥 Added to group: USER_{email}");
        }
        else
        {
            Console.WriteLine("⚠️ Email not found in JWT claims");
        }

        await base.OnConnectedAsync();
    }
}

}
