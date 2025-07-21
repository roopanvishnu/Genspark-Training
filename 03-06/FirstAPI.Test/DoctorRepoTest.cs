using FirstAPI.Contexts;
using Microsoft.EntityFrameworkCore;


namespace FirstAPI.Test;

public class Tests
{
    private ClinicContext _context;
    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ClinicContext>()
                            .UseInMemoryDatabase("TestDb")
                            .Options;
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}