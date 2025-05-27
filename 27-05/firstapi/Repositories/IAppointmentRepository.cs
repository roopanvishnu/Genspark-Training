namespace FirstApi.Repositories;

using FirstApi.Models;
public interface IAppointmentRepository
{
    IEnumerable<Appointment> GetAppointments();
    Appointment AddAppointment(Appointment appointment);
    Appointment UpdateAppointment(int id, Appointment updatedAppointment);
    bool DeleteAppointment(int id);
}