namespace FirstApi.Services;

using FirstApi.Models;
using FirstApi.Repositories;
public class PatientService
{
    private readonly IPatientRepository _patientRepository;

    public PatientService(IPatientRepository patientRepository)
    {
        _patientRepository = patientRepository;
    }

    public IEnumerable<Patient> GetPatients()
    {
        return _patientRepository.GetPatients();
    }

    public Patient AddPatient(Patient patient)
    {
        return _patientRepository.AddPatient(patient);
    }

    public Patient UpdatePatient(int id, Patient updatedPatient)
    {
        return _patientRepository.UpdatePatient(id, updatedPatient);
    }

    public bool DeletePatient(int id)
    {
        return _patientRepository.DeletePatient(id);
    }
}