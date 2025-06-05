using System.Text;
using AutoMapper;
using Castle.Core.Logging;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Mappers;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;
using FirstAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace FirstAPI.Test
{
    public class PatientServiceTest
    {
        private ClinicContext _context;
        Mock<PatientRepository> patientRepositoryMock;
        Mock<UserRepository> userRepositoryMock;
        Mock<EncryptionService> encryptionServiceMock;
        Mock<IMapper> mapperMock;
        Mock<ILogger<PatientService>> loggerMock;
        IPatientService patientService;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("TestDb")
                                .Options;
            _context = new ClinicContext(options);

            patientRepositoryMock = new Mock<PatientRepository>(_context);
            userRepositoryMock = new(_context);
            encryptionServiceMock = new();
            mapperMock = new();
            loggerMock = new();


            patientRepositoryMock.Setup(dr => dr.Add(It.IsAny<Patient>()))
                                            .ReturnsAsync((Patient dto) => new Patient { Id = 1 });
            patientRepositoryMock.Setup(dr => dr.Get(It.IsAny<int>()))
                                            .ReturnsAsync( new Patient { Id = 1 });
            patientRepositoryMock.Setup(dr => dr.GetAll())
                                            .ReturnsAsync( new List<Patient>{
                                                                new Patient { Id = 1 , Name="Test", Email = "test@mail.com"},
                                                                new Patient { Id = 2 , Name="Test", Email = "test2@mail.com"}
                                                            });


            userRepositoryMock.Setup(u => u.Add(It.IsAny<User>()))
                                            .ReturnsAsync((User dto) => new User { Username= "test@mail.com" });

            encryptionServiceMock.Setup(es => es.EncryptData(It.IsAny<EncryptModel>()))
                                            .Returns((EncryptModel model) =>new EncryptModel
                                            {
                                                Data = "password",
                                                EncryptedData = Encoding.UTF8.GetBytes("password"),
                                                HashKey = Encoding.UTF8.GetBytes("key")
                                            });
            mapperMock.Setup(m => m.Map<PatientAddRequestDTO, User>(It.IsAny<PatientAddRequestDTO>()))
                  .Returns(new User { Username= "test@mail.com" });

            patientService = new PatientService(patientRepositoryMock.Object,
                                                            userRepositoryMock.Object,
                                                            encryptionServiceMock.Object,
                                                            loggerMock.Object,
                                                            mapperMock.Object);
        }



        [Test]
        public async Task Test_AddPatient()
        {
            PatientAddRequestDTO dto = new PatientAddRequestDTO
            {
                Name = "Test",
                Email = "test@mail.com",
                Password = "test"
            };
            Patient patient = await patientService.AddPatient(dto);
            Assert.That(patient, Is.Not.Null);
            Assert.That(patient.Id, Is.EqualTo(1));
        }

        [TestCase(1)]
        public async Task Test_GetPatientById(int id)
        {
            Patient? patient = await patientService.GetPatientById(id);
            Assert.That(patient, Is.Not.Null);
        }

        [Test]
        public async Task Test_GetPatients()
        {
            var doctors = await patientService.GetPatients();
            Assert.That(doctors, Is.Not.Null);
            Assert.That(doctors.Count(), Is.EqualTo(2));
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    
    }
}