using FirstAPI.Models.DTOs.DoctorSpecialities;

namespace FirstAPI.Models
{
    namespace FirstAPI.Interfaces
    {
        public interface IDoctorService
        {
            Task<Doctor> GetDoctorById(int id);
            Task<IEnumerable<Doctor>> GetAllDoctors();
            Task<Doctor> AddNewDoctor(Doctor doctor);
            Task<Doctor> UpdateDoctor(Doctor doctor, int id);
            Task<Doctor> DeleteDoctor(int id);

            Task<Doctor> GetDoctorByName(string name);
            Task<ICollection<Doctor>> GetDoctorsBySpeciality(string speciality);
        }
    }
}