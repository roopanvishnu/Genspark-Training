namespace FirstApi.Repositories;

using FirstApi.Models;
public class DoctorRepository : IDoctorRepository
{
    private static List<Doctor> doctors = new List<Doctor>();

    public IEnumerable<Doctor> GetDoctors()
    {
        return doctors;
    }

    public Doctor AddDoctor(Doctor doctor)
    {
        doctors.Add(doctor);
        return doctor;
    }

    public Doctor UpdateDoctor(int id, Doctor updatedDoctor)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor != null)
        {
            doctor.Name = updatedDoctor.Name;
            return doctor;
        }
        return null;
    }

    public bool DeleteDoctor(int id)
    {
        var doctor = doctors.FirstOrDefault(d => d.Id == id);
        if (doctor != null)
        {
            doctors.Remove(doctor);
            return true;
        }
        return false;
    }
}