using System.Text;
using AutoMapper;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Mappers;
using FirstAPI.Misc;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;
using FirstAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FirstAPI.Test
{
    public class DoctorServiceTest
    {
        private ClinicContext _context;
        Mock<DoctorRepository> doctorRepositoryMock;
        Mock<SpecialityRepository> specialityRepositoryMock;
        Mock<DoctorSpecialityRepository> doctorSpecialityRepositoryMock;
        Mock<UserRepository> userRepositoryMock;
        Mock<OtherContextFns> otherContextFunctionitiesMock;
        Mock<EncryptionService> encryptionServiceMock;
        Mock<IMapper> mapperMock;
        Mock<DoctorMapper> doctorMapperMock;
        Mock<SpecialityMapper> specialityMapperMock;
        IDoctorService doctorService;


        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("TestDb")
                                .Options;
            _context = new ClinicContext(options);

            doctorRepositoryMock = new Mock<DoctorRepository>(_context);
            specialityRepositoryMock = new(_context);
            doctorSpecialityRepositoryMock = new(_context);
            userRepositoryMock = new(_context);
            otherContextFunctionitiesMock = new(_context);
            encryptionServiceMock = new();
            mapperMock = new();
            doctorMapperMock = new();
            specialityMapperMock = new();

            otherContextFunctionitiesMock.Setup(ocf => ocf.GetDoctorsBySpeciality(It.IsAny<string>()))
                                        .ReturnsAsync((string speciality) => new List<DoctorBySpecialityResponseDTO>{
                                            new DoctorBySpecialityResponseDTO
                                            {
                                                Dname = "test",
                                                Yoe = 2,
                                                Id=1
                                            }
                                        });
            doctorRepositoryMock.Setup(dr => dr.Add(It.IsAny<Doctor>()))
                                            .ReturnsAsync((Doctor dto) => new Doctor { Id = 1 });
            doctorRepositoryMock.Setup(dr => dr.Get(It.IsAny<int>()))
                                            .ReturnsAsync( new Doctor { Id = 1 });
            doctorRepositoryMock.Setup(dr => dr.GetAll())
                                            .ReturnsAsync( new List<Doctor>{
                                                                new Doctor { Id = 1 , Name="Test", Email = "test@mail.com"},
                                                                new Doctor { Id = 2 , Name="Test", Email = "test2@mail.com"}
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
            mapperMock.Setup(m => m.Map<DoctorAddRequestDTO, User>(It.IsAny<DoctorAddRequestDTO>()))
                  .Returns(new User { Username= "test@mail.com" });

            doctorMapperMock.Setup(dm => dm.MapDoctorAddRequestDoctor(It.IsAny<DoctorAddRequestDTO>()))
                                    .Returns(new Doctor { Id = 1 });

            doctorService = new DoctorService(doctorRepositoryMock.Object,
                                                            specialityRepositoryMock.Object,
                                                            doctorSpecialityRepositoryMock.Object,
                                                            userRepositoryMock.Object,
                                                            encryptionServiceMock.Object,
                                                            otherContextFunctionitiesMock.Object,
                                                            doctorMapperMock.Object,
                                                            specialityMapperMock.Object,
                                                            mapperMock.Object);
        }

        [TestCase("General")]
        public async Task Test_GetDoctorBySpeciality(string speciality)
        {


            //Assert.That(doctorService, Is.Not.Null);
            //Action
            var result = await doctorService.GetDoctorsBySpeciality(speciality);
            //Assert
            Assert.That(result?.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Test_AddDoctor()
        {
            DoctorAddRequestDTO dto = new DoctorAddRequestDTO
            {
                Name = "Test",
                Email = "test@mail.com",
                YearsOfExperience = 8,
                Password = "test"
            };
            Doctor doctor = await doctorService.AddDoctor(dto);
            Assert.That(doctor, Is.Not.Null);
            Assert.That(doctor.Id, Is.EqualTo(1));
        }

        [TestCase(1)]
        public async Task Test_Get(int id)
        {
            Doctor? doctor = await doctorService.GetDoctorById(id);
            Assert.That(doctor, Is.Not.Null);
        }

        [TestCase("test@mail.com")]
        public async Task Test_GetDoctorByEmail(string email)
        {
            Doctor? doctor = await doctorService.GetDoctorByEmail(email);
            Assert.That(doctor, Is.Not.Null);
        }

        [TestCase("Test")]
        public async Task Test_GetDoctorsByName(string name)
        {
            var doctors = await doctorService.GetDoctorsByName(name);
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