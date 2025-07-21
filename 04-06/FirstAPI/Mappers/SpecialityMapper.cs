using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Mappers
{
    public class SpecialityMapper
    {
        public virtual Speciality MapSpecialityAddRequestDoctor(SpecialityAddRequestDTO addRequestDTO)
        {
            return new Speciality{Name = addRequestDTO.Name,Status="Active"};
        }
        public virtual DoctorSpeciality MapDoctorSpeciality(int docId, int specId)
        {
            DoctorSpeciality doctorSpeciality = new DoctorSpeciality
            {
                DoctorId = docId,
                SpecialityId = specId
            };
            return doctorSpeciality;
        }
    } 
}