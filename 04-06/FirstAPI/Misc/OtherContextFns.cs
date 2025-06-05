using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Misc
{
    public class OtherContextFns : IOtherContextFns
    {
        private readonly ClinicContext _clinicContext;
        public OtherContextFns(ClinicContext clinicContext)
        {
            _clinicContext = clinicContext;
        }
        public async virtual Task<ICollection<DoctorBySpecialityResponseDTO>> GetDoctorsBySpeciality(string speciality)
        {
            return await _clinicContext.GetDoctorBySpeciality(speciality);
        }
    }
}