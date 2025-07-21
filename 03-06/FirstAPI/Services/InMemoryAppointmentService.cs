using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Services
{
    public class InMemoryAppointmentService : IAppointmentService
    {
        private readonly List<Appointmnet> _appointments;

        public InMemoryAppointmentService()
        {
            _appointments = new List<Appointmnet>
            {
                new Appointmnet { AppointmnetNumber = "A1", PatientId = 1, DoctorId = 1, AppointmnetDateTime = DateTime.Now.AddDays(1), Status = "Scheduled" },
                new Appointmnet { AppointmnetNumber = "A2", PatientId = 2, DoctorId = 2, AppointmnetDateTime = DateTime.Now.AddDays(2), Status = "Scheduled" },
                new Appointmnet { AppointmnetNumber = "A3", PatientId = 3, DoctorId = 3, AppointmnetDateTime = DateTime.Now.AddDays(3), Status = "Scheduled" },
                new Appointmnet { AppointmnetNumber = "A4", PatientId = 4, DoctorId = 1, AppointmnetDateTime = DateTime.Now.AddDays(4), Status = "Scheduled" },
                new Appointmnet { AppointmnetNumber = "A5", PatientId = 5, DoctorId = 2, AppointmnetDateTime = DateTime.Now.AddDays(5), Status = "Scheduled" }
            };
        }

        public Task<bool> CancelAppointmentAsync(string appointmentNumber, int doctorId)
        {
            var appointment = _appointments.FirstOrDefault(a => a.AppointmnetNumber == appointmentNumber && a.DoctorId == doctorId);
            if (appointment == null) return Task.FromResult(false);

            appointment.Status = "Cancelled";
            return Task.FromResult(true);
        }

        public IEnumerable<Appointmnet> GetAllAppointments()
        {
            return _appointments;
        }
    }
}