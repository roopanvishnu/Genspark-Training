using System;
using AutoMapper;
using FirstAPI.Imterfaces;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Services;

public class PatientService : IPatientService
{
    private readonly IRepository<int, Patient> _patientRepository;
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ILogger<PatientService> _logger;
    private readonly IMapper _mapper;
    public PatientService(
                            IRepository<int, Patient> patientRepository,
                            IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            ILogger<PatientService> logger,
                            IMapper mapper
                        )
    {
        _patientRepository = patientRepository;
        _userRepository = userRepository;
        _encryptionService = encryptionService;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<Patient> GetPatientById(int id)
    {
        return await _patientRepository.Get(id);
    }

    public async Task<ICollection<Patient>> GetPatients()
    {
        var patients = await _patientRepository.GetAll();
        if (patients == null || patients.Count() == 0) throw new Exception("No patients data found");
        return patients.ToList();
    }

    public async Task<Patient> AddPatient(PatientAddRequestDTO addRequestDTO)
    {
        try
        {
            var user = _mapper.Map<PatientAddRequestDTO, User>(addRequestDTO);
            var encryptedPassword = _encryptionService.EncryptData(new EncryptModel { Data = addRequestDTO.Password });
            user.Password = encryptedPassword.EncryptedData;
            user.HashKey = encryptedPassword.HashKey;
            user.Role = "Patient";
            await _userRepository.Add(user);

            var patient = new Patient { Name = addRequestDTO.Name, Age = addRequestDTO.Age, Email = addRequestDTO.Email, Phone = addRequestDTO.Phone };
            patient = await _patientRepository.Add(patient);
            if (patient == null)
            {
                throw new Exception("Creation Failed!");
            }
            return patient;
        }
        catch
        {
            throw;
        }
    }
}
