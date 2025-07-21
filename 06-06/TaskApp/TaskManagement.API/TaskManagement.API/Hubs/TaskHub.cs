using Microsoft.AspNetCore.SignalR;

namespace TaskManagement.API.Hubs
{
    public class TaskHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"client {Context.ConnectionId} connected task hub sucessfully");
            await base.OnConnectedAsync();
        }
        
    }
}