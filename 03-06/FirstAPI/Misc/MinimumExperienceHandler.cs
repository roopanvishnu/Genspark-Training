using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using FirstAPI.Contexts; // Adjust namespace to match where ClinicContext is
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace FirstAPI.Misc;

public class MinimumExperienceHandler : AuthorizationHandler<MinimumExperienceRequirement>
{
    private readonly ClinicContext _context;

    public MinimumExperienceHandler(ClinicContext context)
    {
        _context = context;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        MinimumExperienceRequirement requirement
    )
    {
        var doctorIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (doctorIdClaim == null || !int.TryParse(doctorIdClaim, out var doctorId))
            return;

        var doctor = await _context.Doctors.FirstOrDefaultAsync(d => d.Id == doctorId);

        if (doctor != null && doctor.YearsOfExperience >= requirement.MinimumYears)
        {
            context.Succeed(requirement);
        }
    }
}