using FirstApi.Models;
using Microsoft.EntityFrameworkCore;

namespace firstapi.Context;

public class ClinicContext: DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    { 
        optionsBuilder.UseNpgsql("User ID=roopanvishnu;Password=1234;Host=localhost;Port=5432;Database=Firstapi;");
    } 
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
}
