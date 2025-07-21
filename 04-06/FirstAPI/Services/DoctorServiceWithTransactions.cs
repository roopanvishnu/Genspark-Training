using System.Formats.Tar;
using System.Threading.Tasks;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Misc;
using FirstAPI.Mappers;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;


namespace FirstAPI.Services
{
    public class DoctorServiceWithTransaction : IDoctorService
    {
        private readonly ClinicContext _clinicContext;
        private readonly DoctorMapper _doctorMapper;
        private readonly SpecialityMapper _specialityMapper;

        public DoctorServiceWithTransaction(ClinicContext clinicContext, DoctorMapper doctorMapper, SpecialityMapper specialityMapper)
        {
            _clinicContext = clinicContext;
            _doctorMapper = doctorMapper;
            _specialityMapper = specialityMapper;
        }
        public async Task<Doctor> AddDoctor(DoctorAddRequestDTO doctor)
        {
            using var transaction = await _clinicContext.Database.BeginTransactionAsync();
            var newDoctor = _doctorMapper.MapDoctorAddRequestDoctor(doctor);

            try
            {
                await _clinicContext.AddAsync(newDoctor);
                await _clinicContext.SaveChangesAsync();
                if (doctor.Specialities!=null && doctor.Specialities.Count() > 0)
                {
                    var existingSpecialities = await _clinicContext.specialities.ToListAsync();
                    foreach (var item in doctor.Specialities)
                    {

                        var speciality = await _clinicContext.specialities.FirstOrDefaultAsync(s => s.Name.ToLower() == item.Name.ToLower());
                        if (speciality == null)
                        {
                            speciality = _specialityMapper.MapSpecialityAddRequestDoctor(item);
                            await _clinicContext.AddAsync(speciality);
                            await _clinicContext.SaveChangesAsync();
                        }
                        var doctorSpeciality = _specialityMapper.MapDoctorSpeciality(newDoctor.Id, speciality.Id);
                        await _clinicContext.AddAsync(doctorSpeciality);

                    }
                    await _clinicContext.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
            return newDoctor;
        }

        public Task<Doctor> GetDoctByName(string name)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetDoctorByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Doctor?> GetDoctorById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Doctor>?> GetDoctorsByName(string name)
        {
            throw new NotImplementedException();
        }


        Task<IEnumerable<DoctorBySpecialityResponseDTO>?> IDoctorService.GetDoctorsBySpeciality(string speciality)
        {
            throw new NotImplementedException();
        }
    }
}