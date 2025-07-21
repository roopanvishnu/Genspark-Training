using System;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IDoctorService
{
    public Task<Doctor?> GetDoctorById(int id);
    public Task<Doctor?> GetDoctorByEmail(string email);
    public Task<IEnumerable<Doctor>?> GetDoctorsByName(string name);
    public Task<IEnumerable<DoctorBySpecialityResponseDTO>?> GetDoctorsBySpeciality(string speciality);
    public Task<Doctor> AddDoctor(DoctorAddRequestDTO doctor);
}
