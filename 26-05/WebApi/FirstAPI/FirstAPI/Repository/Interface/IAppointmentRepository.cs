using FirstAPI.Models;

namespace FirstAPI.Repository.Interface;

public interface IAppointmentRepository
{
    IEnumerable<Appointment> GetAllAppointments();
    Appointment? GetAppointmentById(int id);
    void CreateAppointment(Appointment appointment);
    void DeleteAppointment(int id);
}