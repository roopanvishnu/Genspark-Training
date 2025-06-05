using System.Threading.Tasks;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FirstAPI.Test;

public class PatientRepoTest
{
    private PatientRepository _patientRepository;
    private ClinicContext _clinicContext;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>().UseInMemoryDatabase("TestDb").Options;
        _clinicContext = new ClinicContext(options);
        _patientRepository = new PatientRepository(_clinicContext);
    }

    [Test]
    public async Task Test_Add()
    {
        Patient patient = new Patient
        {
            Name = "Test",
            Age = 56,
            Email = "test@mail.com",
            Phone = "9876543212"
        };
        patient = await _patientRepository.Add(patient);
        Assert.That(patient, Is.Not.Null);
        Assert.That(patient.Id, Is.EqualTo(1));
    }
    [Test]
    public async Task Test_Update()
    {
        Patient patient = new Patient
        {
            Name = "Test",
            Age = 67,
            Email = "test@mail.com",
            Phone = "9876543212"
        };

        patient = await _patientRepository.Update(1,patient);
        patient = await _patientRepository.Get(1);
        Assert.That(patient, Is.Not.Null);
        Assert.That(patient.Id, Is.EqualTo(1));
        Assert.That(patient.Age, Is.EqualTo(67));
    }
    [Test]
    public async Task Test_Get()
    {
        Patient patient = await _patientRepository.Get(1);
        Assert.That(patient, Is.Not.Null);
        Assert.That(patient.Id, Is.EqualTo(1));
        // Assert.That(patient.Age, Is.EqualTo(67));

        Assert.ThrowsAsync<Exception>(() => _patientRepository.Get(2));
        Assert.That(() => _patientRepository.Get(2), Throws.TypeOf<Exception>());
    }

    [Test]
    public async Task Test_GetAll()
    {
        var patients = await _patientRepository.GetAll();
        Assert.That(patients, Is.Not.Null);
        Assert.That(patients.Count(), Is.EqualTo(1));
    }
    [TearDown]
    public async Task TearDown()
    {
        await _clinicContext.DisposeAsync();
    }
}
