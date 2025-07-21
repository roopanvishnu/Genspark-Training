namespace FirstApi.Services;

using FirstApi.Models;
using FirstApi.Repositories;
public class DoctorService
{
    private readonly IDoctorRepository _doctorRepository;

    public DoctorService(IDoctorRepository doctorRepository)
    {
        _doctorRepository = doctorRepository;
    }

    public IEnumerable<Doctor> GetDoctors()
    {
        return _doctorRepository.GetDoctors();
    }

    public Doctor AddDoctor(Doctor doctor)
    {
        return _doctorRepository.AddDoctor(doctor);
    }

    public Doctor UpdateDoctor(int id, Doctor updatedDoctor)
    {
        return _doctorRepository.UpdateDoctor(id, updatedDoctor);
    }

    public bool DeleteDoctor(int id)
    {
        return _doctorRepository.DeleteDoctor(id);
    }
}