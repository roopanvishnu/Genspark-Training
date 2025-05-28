using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository<int, Patient> _patientRepository;

        public PatientService(IRepository<int, Patient> patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<Patient> GetPatientById(int id)
        {
            var patient = await _patientRepository.Get(id);
            if (patient == null)
                throw new Exception("Patient not found");
            return patient;
        }

        public async Task<IEnumerable<Patient>> GetAllPatients()
        {
            return await _patientRepository.GetAll();
        }

        public async Task<Patient> AddNewPatient(Patient patient)
        {
            // Simple validation example
            if (string.IsNullOrWhiteSpace(patient.Name))
                throw new Exception("Patient name is required");
            if (patient.Age <= 0)
                throw new Exception("Invalid patient age");

            // You could also check for existing patient email uniqueness, etc.

            return await _patientRepository.Add(patient);
        }

        public async Task<Patient> UpdatePatient(Patient patient, int id)
        {
            var existingPatient = await _patientRepository.Get(id);
            if (existingPatient == null)
                throw new Exception("Patient not found");

            // Update properties
            existingPatient.Name = patient.Name;
            existingPatient.Age = patient.Age;
            existingPatient.Email = patient.Email;
            existingPatient.Phone = patient.Phone;

            return await _patientRepository.Update(id, existingPatient);
        }

        public async Task<Patient> DeletePatient(int id)
        {
            var patient = await _patientRepository.Get(id);
            if (patient == null)
                throw new Exception("Patient not found");

            return await _patientRepository.Delete(id);
        }
    }
}
