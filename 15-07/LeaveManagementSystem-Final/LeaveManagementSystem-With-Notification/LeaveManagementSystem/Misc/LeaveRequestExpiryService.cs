using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Interfaces;

namespace LeaveManagementSystem.Misc {
    public class LeaveRequestExpiryService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public LeaveRequestExpiryService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var repository = scope.ServiceProvider.GetRequiredService<IRepository<Guid, LeaveRequest>>();
                    var currentDate = DateTime.UtcNow;

                    var pendingRequests = await repository.GetAll();
                    var toExpire = pendingRequests
                        .Where(r => r.Status == "Pending" && r.StartDate.Date <= currentDate.Date)
                        .ToList();

                    foreach (var request in toExpire)
                    {
                        request.Status = "Auto-Rejected"; 
                        request.UpdatedAt = DateTime.UtcNow;

                        await repository.Update(request.Id, request);
                    }
                }

                await Task.Delay(TimeSpan.FromHours(24), stoppingToken); 
            }
        }
    }
}
