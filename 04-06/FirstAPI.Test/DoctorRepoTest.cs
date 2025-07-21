using System.Threading.Tasks;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FirstAPI.Test;

public class DoctorRepoTest
{
    private DoctorRepository _DoctorRepository;
    private ClinicContext _clinicContext;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>().UseInMemoryDatabase("TestDb").Options;
        _clinicContext = new ClinicContext(options);
        _DoctorRepository = new DoctorRepository(_clinicContext);
    }

    [Test]
    public async Task Test_Add()
    {
        Doctor doctor = new Doctor
        {
            Name = "Test",
            Email = "test@mail.com",
            YearsOfExperience=8
        };
        doctor = await _DoctorRepository.Add(doctor);
        Assert.That(doctor, Is.Not.Null);
        Assert.That(doctor.Id, Is.EqualTo(1));
    }
    [Test]
    public async Task Test_Update()
    {
        Doctor doctor = new Doctor
        {
            Name = "Test",
            Email = "test@mail.com",
            YearsOfExperience = 10
        };

        doctor = await _DoctorRepository.Update(1, doctor);
        Assert.That(doctor, Is.Not.Null);
        Assert.That(doctor.Id, Is.EqualTo(1));
        Assert.That(doctor.YearsOfExperience, Is.EqualTo(10));
    }
    [Test]
    public async Task Test_Get()
    {
        Doctor doctor = await _DoctorRepository.Get(1);
        Assert.That(doctor, Is.Not.Null);
        Assert.That(doctor.Id, Is.EqualTo(1));

        Assert.ThrowsAsync<Exception>(() => _DoctorRepository.Get(2));
        Assert.That(() => _DoctorRepository.Get(2), Throws.TypeOf<Exception>());
    }

    [Test]
    public async Task Test_GetAll()
    {
        var doctors = await _DoctorRepository.GetAll();
        Assert.That(doctors, Is.Not.Null);
        Assert.That(doctors.Count(), Is.EqualTo(1));
    }
    [TearDown]
    public async Task TearDown()
    {
        await _clinicContext.DisposeAsync();
    }
}
