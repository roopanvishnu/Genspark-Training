using System.Security.Claims;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Authorizations
{
    public class MinExperienceAuthorizationHandler : AuthorizationHandler<MinExperienceAuthorizationRequirement>
    {
        private readonly IDoctorService _doctorService;
        public MinExperienceAuthorizationHandler(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MinExperienceAuthorizationRequirement requirement)
        {
            var email = context.User.FindFirst(ClaimTypes.NameIdentifier);
            var role = context.User.FindFirst(ClaimTypes.Role)?.Value;
            if (role == null || role != "Doctor")
            {
                context.Fail();
                return;
            }
            if (email == null)
            {
                context.Fail();
                return;
            }
            Doctor? doctor = await _doctorService.GetDoctorByEmail(email.Value);
            if (doctor!=null && doctor.YearsOfExperience >= requirement.MinExp)
            {
                context.Succeed(requirement);
                return;
            }
            else
            {
                context.Fail();
                return;
            }
        }
    }
}