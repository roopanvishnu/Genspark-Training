using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace LeaveManagementSystem.Authorization {
    public class CanUpdateUserHandler : AuthorizationHandler<CanUpdateUserRequirement, Guid>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanUpdateUserRequirement requirement, Guid userIdToUpdate)
        {
            var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var roleClaim = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (userIdClaim == null || roleClaim == null)
                return Task.CompletedTask;

            if (roleClaim == "HR" || userIdClaim == userIdToUpdate.ToString())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
