using AppointmentSystem.Models;

namespace AppointmentSystem.Repositories;

public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new();
    private int _nextId = 1;

    public Appointment Add(Appointment appointment)
    {
        appointment.Id = _nextId++;
        _appointments.Add(appointment);
        return appointment;
    }

    public List<Appointment> GetAll()
    {
        return _appointments;
    }
}
