using FirstAPI.Models;

namespace FirstAPI.Interfaces
{
    public interface IAppointmentService
    {
        Task<bool> CancelAppointmentAsync(string appointmentNumber, int doctorId);
        IEnumerable<Appointmnet> GetAllAppointments();
    }
}