using FirstAPI.Models;

namespace FirstAPI.Interfaces;

public interface IPatientService
{
    Task<Patient> GetPatientById(int id);
    Task<IEnumerable<Patient>> GetAllPatients();
    Task<Patient>AddNewPatient(Patient patient);
    Task<Patient> UpdatePatient(Patient patient, int id);
    Task<Patient> DeletePatient(int id);
}