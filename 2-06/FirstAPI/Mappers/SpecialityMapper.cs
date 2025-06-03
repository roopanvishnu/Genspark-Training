using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Mappers
{
    public class SpecialityMapper
    {
        public static Speciality MapSpecialityAddRequestDoctor(SpecialityAddRequestDTO addRequestDTO)
        {
            return new Speciality{Name = addRequestDTO.Name,Status="Active"};
        }
        public static DoctorSpeciality MapDoctorSpeciality(int docId, int specId)
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