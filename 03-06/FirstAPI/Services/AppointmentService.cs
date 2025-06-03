using FirstAPI.Contexts;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FirstAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ClinicContext _context;

        public AppointmentService(ClinicContext context)
        {
            _context = context;
        }

        public async Task<bool> CancelAppointmentAsync(string appointmentNumber, int doctorId)
        {
            var appointment = await _context.Appointmnets
                .Include(a => a.Doctor)
                .SingleOrDefaultAsync(a => a.AppointmnetNumber == appointmentNumber && a.DoctorId == doctorId);

            if (appointment == null) return false;

            appointment.Status = "Cancelled";
            await _context.SaveChangesAsync();

            return true;
        }

        public IEnumerable<Appointmnet> GetAllAppointments()
        {
            return _context.Appointmnets.ToList();
        }
    }
}