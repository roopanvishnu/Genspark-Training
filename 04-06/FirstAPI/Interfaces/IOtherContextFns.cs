using System;
using FirstAPI.Contexts;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IOtherContextFns
{
    public Task<ICollection<DoctorBySpecialityResponseDTO>> GetDoctorsBySpeciality(string speciality);
}
