namespace FirstApi.Repositories;

using FirstApi.Models;
public class PatientRepository : IPatientRepository
{
    private static List<Patient> patients = new List<Patient>();

    public IEnumerable<Patient> GetPatients()
    {
        return patients;
    }

    public Patient AddPatient(Patient patient)
    {
        patients.Add(patient);
        return patient;
    }

    public Patient UpdatePatient(int id, Patient updatedPatient)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient != null)
        {
            patient.Name = updatedPatient.Name;
            patient.Age = updatedPatient.Age;
            patient.Disease = updatedPatient.Disease;
            return patient;
        }
        return null;
    }

    public bool DeletePatient(int id)
    {
        var patient = patients.FirstOrDefault(p => p.Id == id);
        if (patient != null)
        {
            patients.Remove(patient);
            return true;
        }
        return false;
    }
}