using System;
using System.Security.Cryptography;
using AutoMapper;
using FirstAPI.Imterfaces;
using FirstAPI.Interfaces;
using FirstAPI.Mappers;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Services;

public class DoctorService : IDoctorService
{
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Speciality> _specialityRepository;
    private readonly IRepository<int, DoctorSpeciality> _doctorSpecialityRepository;
    private readonly IOtherContextFns _otherContextFns;
    private readonly IMapper _mapper;
    private readonly IRepository<string, User> _userRepository;
    private readonly IEncryptionService _encryptionService;

    public DoctorService(
                            IRepository<int, Doctor> doctorRepository,
                            IRepository<int, Speciality> specialityRepository,
                            IRepository<int, DoctorSpeciality> doctorSpecialityRepository,
                            IRepository<string, User> userRepository,
                            IEncryptionService encryptionService,
                            IOtherContextFns otherContextFns,
                            IMapper mapper
                        )
    {
        _doctorRepository = doctorRepository;
        _specialityRepository = specialityRepository;
        _doctorSpecialityRepository = doctorSpecialityRepository;
        _otherContextFns = otherContextFns;
        _mapper = mapper;
        _encryptionService = encryptionService;
        _userRepository = userRepository;
    }


    public async Task<Doctor> AddDoctor(DoctorAddRequestDTO doctor)
    {
        // Doctor newDoc = new Doctor
        // {
        //     Name = doctor.Name,
        //     YearsOfExperience = doctor.YearsOfExperience,
        //     Status = "Created"
        // };
        // if (doctor.Specialities != null && doctor.Specialities.Count() > 0)
        // {
        //     IEnumerable<Speciality> specialities = await _specialityRepository.GetAll();
        //     foreach (var speciality in doctor.Specialities)
        //     {
        //         Speciality? spec = specialities.FirstOrDefault(s => s.Name == speciality.Name);
        //         if (spec == null)
        //         {
        //             spec = await _specialityRepository.Add(new Speciality { Name = speciality.Name, Status = "Created" });
        //         }
        //         await _doctorSpecialityRepository.Add(new DoctorSpeciality { DoctorId = doc.Id, SpecialityId = spec.Id });
        //     }
        // }

        try
        {
            var user = _mapper.Map<DoctorAddRequestDTO,User>(doctor);
            var EncryptedData = _encryptionService.EncryptData(
                new EncryptModel { Data = doctor.Password }
            );
            user.Password = EncryptedData.EncryptedData;
            user.HashKey = EncryptedData.HashKey;
            user.Role = "Doctor";
            await _userRepository.Add(user);
            Doctor newDoc = DoctorMapper.MapDoctorAddRequestDoctor(doctor);
            Doctor doc = await _doctorRepository.Add(newDoc);
            if (doc == null)
                throw new Exception("Could not add doctor");
            if (doctor.Specialities != null && doctor.Specialities.Count > 0)
            {
                int[] specIds = await MapAndAddSpeciality(doctor);
                for (int i = 0; i < specIds.Length; i++)
                {
                    DoctorSpeciality ds = SpecialityMapper.MapDoctorSpeciality(doc.Id, specIds[i]);
                    await _doctorSpecialityRepository.Add(ds);
                }
            }
            return doc;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<int[]> MapAndAddSpeciality(DoctorAddRequestDTO doctor)
    {
        IEnumerable<Speciality> specialities;
        int[] specIds = new int[doctor.Specialities!.Count()];
        try
        {
             specialities = await _specialityRepository.GetAll();
        }
        catch
        {
            specialities = new List<Speciality>();
        }
        int count = 0;

            foreach (var speciality in doctor.Specialities!)
            {
                Speciality? spec = specialities.FirstOrDefault(s => s.Name == speciality.Name);
                if (spec == null)
                {
                    spec = await _specialityRepository.Add(SpecialityMapper.MapSpecialityAddRequestDoctor(speciality));
                }
                specIds[count++] = spec.Id;
            }

        return specIds;
    }
    public async Task<Doctor?> GetDoctorById(int id)
    {
        Doctor? doc = await _doctorRepository.Get(id);
        if (doc == null)
        {
            throw new Exception("No doctor found");
        }
        return doc;
    }
    
    public async Task<IEnumerable<Doctor>?> GetDoctorsByName(string name)
    {
        try
        {
            IEnumerable<Doctor> docList = await _doctorRepository.GetAll();
            docList = docList.Where(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (docList == null || docList.Count() == 0)
            {
                throw new Exception("No doctors found");
            }
            else
                return docList;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<IEnumerable<DoctorBySpecialityResponseDTO>?> GetDoctorsBySpeciality(string speciality)
    {
        // IEnumerable<Speciality> specs = await _specialityRepository.GetAll();
        // Speciality? spec = specs.FirstOrDefault(d => d.Name == speciality);
        // if (spec == null)
        // {
        //     System.Console.WriteLine("No specified speciality exists");
        //     return null;
        // }
        // IEnumerable<DoctorSpeciality> docspecs = await _doctorSpecialityRepository.GetAll();
        // docspecs = docspecs.Where(d => d.SpecialityId == spec.Id);
        // if (docspecs == null || docspecs.Count() == 0)
        // {
        //     System.Console.WriteLine("No doctor with specified speciality exists");
        //     return null;
        // }
        // IEnumerable<int> docids = docspecs.Select(d => d.DoctorId).Distinct();
        // IEnumerable<Doctor> docs= await _doctorRepository.GetAll();
        // docs = docs.Where(d => docids.Contains(d.Id) );
        // return docs;

        var result = await _otherContextFns.GetDoctorsBySpeciality(speciality);
        return result;

    }
}
