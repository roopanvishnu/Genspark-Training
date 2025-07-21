namespace FirstApi.Repositories;

using FirstApi.Models;
public interface IPatientRepository
{
    IEnumerable<Patient> GetPatients();
    Patient AddPatient(Patient patient);
    Patient UpdatePatient(int id, Patient updatedPatient);
    bool DeletePatient(int id);
}