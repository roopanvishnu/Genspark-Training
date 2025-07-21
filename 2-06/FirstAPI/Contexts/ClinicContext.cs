using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FirstAPI.Contexts
{
    public class ClinicContext : DbContext
    {
        public ClinicContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Patient> patients { get; set; }
        public DbSet<Doctor> doctors { get; set; }

        public DbSet<Speciality> specialities { get; set; }
        public DbSet<DoctorSpeciality> doctorSpecialities { get; set; }
        public DbSet<Appointment> appointments { get; set; }

        public DbSet<User> users { get; set; }
        
        public DbSet<DoctorBySpecialityResponseDTO> DoctorBySpeciality { get; set; }


        public async Task<List<DoctorBySpecialityResponseDTO>> GetDoctorBySpeciality(string speciality)
        {
            return await this.Set<DoctorBySpecialityResponseDTO>()
                                        .FromSqlInterpolated($"SELECT * FROM fn_GetDoctorsBySpeciality({speciality})")
                                        .ToListAsync();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>().HasOne(p => p.User)
                                        .WithOne(u => u.Patient)
                                        .HasForeignKey<Patient>(p => p.Email)
                                        .HasConstraintName("FK_User_Patient")
                                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Doctor>().HasOne(d => d.User)
                                        .WithOne(u => u.Doctor)
                                        .HasForeignKey<Doctor>(d => d.Email)
                                        .HasConstraintName("FK_User_Doctor")
                                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmentNumber).HasName("PK_AppointmentNumber");

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Patient)
                        .WithMany(p => p.Appointments)
                        .HasForeignKey(a => a.PatientId)
                        .HasConstraintName("FK_Appointment_Patient")
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                        .HasOne(a => a.Doctor)
                        .WithMany(d => d.Appointments)
                        .HasForeignKey(a => a.DoctorId)
                        .HasConstraintName("FK_Appointment_Doctor")
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>().HasKey(ds => ds.SerialNumber).HasName("PK_DoctorSpeciality");
            modelBuilder.Entity<DoctorSpeciality>()
                        .HasOne(ds => ds.Doctor)
                        .WithMany(d => d.DoctorSpecialities)
                        .HasForeignKey(ds => ds.DoctorId)
                        .HasConstraintName("FK_DoctorSpeciality_Doctor")
                        .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DoctorSpeciality>()
                        .HasOne(ds => ds.Speciality)
                        .WithMany(s => s.DoctorSpecialities)
                        .HasConstraintName("FK_DoctorSpeciality_Speciality")
                        .OnDelete(DeleteBehavior.Restrict);

        }
        
    }
}