using AutoMapper;
using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Mappers;
using FirstAPI.Misc;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;
using FirstAPI.Services;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FirstAPI.Test
{
    public class MoqUnitDoctorServiceTest
    {
        private ClinicContext _context;
        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<ClinicContext>()
                                .UseInMemoryDatabase("UnitTestDb")
                                .Options;
            _context = new ClinicContext(options);
        }

        [TestCase("General")]
        public async Task TestGetDoctorBySpeciality(string speciality)
        {
            Mock<DoctorRepository> doctorRepositoryMock = new Mock<DoctorRepository>(_context);
            Mock<SpecialityRepository> specialityRepositoryMock = new(_context);
            Mock<DoctorSpecialityRepository> doctorSpecialityRepositoryMock = new(_context);
            Mock<UserRepository> userRepositoryMock = new(_context);
            Mock<OtherContextFns> otherContextFunctionitiesMock = new(_context);
            Mock<EncryptionService> encryptionServiceMock = new();
            Mock<IMapper> mapperMock = new();
            Mock<DoctorMapper> doctorMapperMock = new();
            Mock<SpecialityMapper> specialityMapperMock = new();

            otherContextFunctionitiesMock.Setup(ocf => ocf.GetDoctorsBySpeciality(speciality))
                                        .ReturnsAsync((string speciality) => new List<DoctorBySpecialityResponseDTO>{
                                            new DoctorBySpecialityResponseDTO
                                            {
                                                Dname = "test",
                                                Yoe = 2,
                                                Id=1
                                            }
                                        });
            IDoctorService doctorService = new DoctorService(doctorRepositoryMock.Object,
                                                            specialityRepositoryMock.Object,
                                                            doctorSpecialityRepositoryMock.Object,
                                                            userRepositoryMock.Object,
                                                            encryptionServiceMock.Object,
                                                            otherContextFunctionitiesMock.Object,
                                                            doctorMapperMock.Object,
                                                            specialityMapperMock.Object,
                                                            mapperMock.Object);


            //Assert.That(doctorService, Is.Not.Null);
            //Action
            var result = await doctorService.GetDoctorsBySpeciality(speciality);
            //Assert
            Assert.That(result?.Count(), Is.EqualTo(1));
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    
    }
}