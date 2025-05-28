using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs.DoctorSpecialities;
using FirstAPI.Models.FirstAPI.Interfaces;

namespace FirstAPI.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IRepository<int, Doctor> _doctorRepository;
        private readonly IRepository<int, Speciality> _specialityRepository;
        private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;

        public DoctorService(
            IRepository<int, Doctor> doctorRepository,
            IRepository<int, Speciality> specialityRepository,
            IRepository<int, DoctorSpeciality> doctorSpecialityRepository)
        {
            _doctorRepository = doctorRepository;
            _specialityRepository = specialityRepository;
            _doctorSpecialityRepository = doctorSpecialityRepository;
        }

        public async Task<Doctor> GetDoctorById(int id)
        {
            var doctor = await _doctorRepository.Get(id);
            if (doctor == null) throw new Exception("Doctor not found");
            return doctor;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctors()
        {
            return await _doctorRepository.GetAll();
        }

        public async Task<Doctor> AddNewDoctor(Doctor doctor)
        {
            var allDoctors = await _doctorRepository.GetAll();
            if (allDoctors.Any(d => d.Name.Equals(doctor.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("Doctor with this name already exists");
            }

            var newDoctor = new Doctor
            {
                Name = doctor.Name,
                YearsOfExperience = doctor.YearsOfExperience,
                Status = doctor.Status ?? "Active" // or some default value
            };

            return await _doctorRepository.Add(newDoctor);
        }

        public async Task<Doctor> AddNewDoctor(DoctorAddRequestDto dto)
        {
            var allDoctors = await _doctorRepository.GetAll();
            if (allDoctors.Any(d => d.Name.Equals(dto.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new Exception("A doctor with this name already exists.");
            }

            var doctor = new Doctor
            {
                Name = dto.Name,
                YearsOfExperience = dto.YearsOfExperience,
                Status = "Active"  
            };

            var addedDoctor = await _doctorRepository.Add(doctor);

            if (dto.Specialities != null && dto.Specialities.Any())
            {
                foreach (var specialityDto in dto.Specialities)
                {
                    var existingSpecialities = await _specialityRepository.GetAll();
                    var speciality = existingSpecialities.FirstOrDefault(s => s.Name.Equals(specialityDto.Name, StringComparison.OrdinalIgnoreCase));
                    
                    if (speciality == null)
                    {
                        speciality = await _specialityRepository.Add(new Speciality
                        {
                            Name = specialityDto.Name,
                            Status = "Active" 
                        });
                    }

                    var doctorSpeciality = new DoctorSpeciality
                    {
                        DoctorId = addedDoctor.Id,
                        SpecialityId = speciality.Id
                    };

                    await _doctorSpecialityRepository.Add(doctorSpeciality);
                }
            }

            return addedDoctor;
        }

        public async Task<Doctor> UpdateDoctor(Doctor doctor, int id)
        {
            var existingDoctor = await _doctorRepository.Get(id);
            if (existingDoctor == null) throw new Exception("Doctor not found");

            var allDoctors = await _doctorRepository.GetAll();
            if (allDoctors.Any(d => d.Name.Equals(doctor.Name, StringComparison.OrdinalIgnoreCase) && d.Id != id))
            {
                throw new Exception("Another doctor with this name already exists.");
            }

            return await _doctorRepository.Update(id, doctor);
        }
        public async Task<Doctor> DeleteDoctor(int id)
        {
            var existingDoctor = await _doctorRepository.Get(id);
            if (existingDoctor == null) throw new Exception("Doctor not found");

            return await _doctorRepository.Delete(id);
        }

        public async Task<Doctor> GetDoctorByName(string name)
        {
            var doctors = await _doctorRepository.GetAll();
            var doctor = doctors.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (doctor == null) throw new Exception("Doctor not found");
            return doctor;
        }

        public async Task<ICollection<Doctor>> GetDoctorsBySpeciality(string specialityName)
        {
            var specialities = await _specialityRepository.GetAll();
            var speciality = specialities.FirstOrDefault(s => s.Name.Equals(specialityName, StringComparison.OrdinalIgnoreCase));

            if (speciality == null) throw new Exception("Speciality not found");

            var doctorSpecialities = await _doctorSpecialityRepository.GetAll();
            var doctorIds = doctorSpecialities
                .Where(ds => ds.SpecialityId == speciality.Id)
                .Select(ds => ds.DoctorId)
                .Distinct();

            var doctors = await _doctorRepository.GetAll();
            return doctors.Where(d => doctorIds.Contains(d.Id)).ToList();
        }
    }
}
