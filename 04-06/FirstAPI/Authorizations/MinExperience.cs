using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Authorizations
{
    public class MinExperienceAuthorizationRequirement : IAuthorizationRequirement
    {
        public int MinExp { get; set; }
        public MinExperienceAuthorizationRequirement(int exp)
        {
            MinExp = exp;
        } 
    }
}