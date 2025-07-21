// Providers/CustomUserIdProvider.cs
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace LeaveManagementSystem.Providers {
public class CustomUserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst("nameid")?.Value; // 🔥 FIXED: use "nameid" explicitly
    }
}


}