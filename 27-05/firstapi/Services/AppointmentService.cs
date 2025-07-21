namespace FirstApi.Services;
using FirstApi.Models;
using FirstApi.Repositories;
public class AppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;
    }

    public IEnumerable<Appointment> GetAppointments()
    {
        return _appointmentRepository.GetAppointments();
    }

    public Appointment AddAppointment(Appointment appointment)
    {
        return _appointmentRepository.AddAppointment(appointment);
    }

    public Appointment UpdateAppointment(int id, Appointment updatedAppointment)
    {
        return _appointmentRepository.UpdateAppointment(id, updatedAppointment);
    }

    public bool DeleteAppointment(int id)
    {
        return _appointmentRepository.DeleteAppointment(id);
    }
}