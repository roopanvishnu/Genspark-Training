using System;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IPatientService
{
    public Task<Patient> GetPatientById(int id);
    public Task<ICollection<Patient>> GetPatients();
    public Task<Patient> AddPatient(PatientAddRequestDTO addRequestDTO);
}
