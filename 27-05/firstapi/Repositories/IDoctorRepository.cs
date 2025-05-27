namespace FirstApi.Repositories;

using FirstApi.Models;
public interface IDoctorRepository
{
    IEnumerable<Doctor> GetDoctors();
    Doctor AddDoctor(Doctor doctor);
    Doctor UpdateDoctor(int id, Doctor updatedDoctor);
    bool DeleteDoctor(int id);
}