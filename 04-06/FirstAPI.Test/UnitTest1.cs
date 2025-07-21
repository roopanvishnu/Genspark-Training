using System.Text;
using System.Threading.Tasks;
using FirstAPI.Contexts;
using FirstAPI.Models;
using FirstAPI.Repositories;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Test;

public class Tests
{
    private ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        DbContextOptions options = new DbContextOptionsBuilder<ClinicContext>()
                                                    .UseInMemoryDatabase("UnitTestDb")
                                                    .Options;
        _context = new ClinicContext(options);
    }

    [Test]
    public async Task AddDoctor()
    {
        // arrange
        var _doctorRepository = new DoctorRepository(_context);
        var _userRepository = new UserRepository(_context);

        var email = "test@mail.com";
        var password = Encoding.UTF8.GetBytes("test");
        var hashKey = Guid.NewGuid().ToByteArray();


        User user = new User
        {
            Username = email,
            Role = "Doctor",
            Password = password,
            HashKey = hashKey
        };
        user = await _userRepository.Add(user);

        Doctor doctor = new Doctor
        {
            Email = email,
            Name = "Test",
            YearsOfExperience = 10
        };

        //action
        doctor = await _doctorRepository.Add(doctor);


        //assert
        Assert.That(doctor, Is.Not.Null, "User not added");
        Assert.That(doctor.Email, Is.EqualTo(user.Username));
        Assert.That(doctor.Id, Is.EqualTo(1));
        // Assert.Pass();
    }

    [TestCase(3)]
    public async Task GetDoctorException(int id)
    {
        // arrange
        var _doctorRepository = new DoctorRepository(_context);
        var _userRepository = new UserRepository(_context);

        var email = "test1@mail.com";
        var password = Encoding.UTF8.GetBytes("test1");
        var hashKey = Guid.NewGuid().ToByteArray();


        User user = new User
        {
            Username = email,
            Role = "Doctor",
            Password = password,
            HashKey = hashKey
        };
        user = await _userRepository.Add(user);

        Doctor doctor = new Doctor
        {
            Email = email,
            Name = "Test1",
            YearsOfExperience = 10
        };

        doctor = await _doctorRepository.Add(doctor);
        //action
        // doctor = await _doctorRepository.Get(id);


        //assert
        Assert.That(() => _doctorRepository.Get(id),Throws.TypeOf<Exception>());
        Exception ex = Assert.ThrowsAsync<Exception>(() => _doctorRepository.Get(id));
        Assert.That(ex.Message, Is.EqualTo("No data found"));
        // Assert.Pass();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
    }
}
