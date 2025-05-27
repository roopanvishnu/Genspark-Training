using FirstAPI.Models;

namespace FirstAPI.Services.Interfaces;

public interface IAppointmentService
{
    IEnumerable<Appointment> GetAppointments();
    Appointment? GetAppointmentById(int id);
    void CreateAppointment(Appointment appointment);
    void DeleteAppointment(int id);
}