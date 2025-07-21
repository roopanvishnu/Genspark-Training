using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Misc;

public class MinimumExperienceRequirement : IAuthorizationRequirement
{
    public float MinimumYears { get; }

    public MinimumExperienceRequirement(float minimumYears)
    {
        MinimumYears = minimumYears;
    }
}