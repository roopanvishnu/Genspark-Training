using FirstAPI.Models;
using FirstAPI.Repository.Interface;

namespace FirstAPI.Repository;

public class AppointmentRepository:IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new();
    
    public IEnumerable<Appointment> GetAllAppointments()
    {
        return _appointments;
    }

    public Appointment? GetAppointmentById(int id)
    {
        var existingAppointment = _appointments.FirstOrDefault(x => x.Id == id);
        return existingAppointment;
    }

    public void CreateAppointment(Appointment appointment)
    {
        appointment.Id = _appointments.Count + 1;
        _appointments.Add(appointment);
    }

    public void DeleteAppointment(int id)
    {
        var existingAppointment = _appointments.FirstOrDefault(x => x.Id == id);
        _appointments.Remove(existingAppointment);
    }
}