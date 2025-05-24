using AppointmentSystem.Models;

namespace AppointmentSystem.Repositories;

public interface IAppointmentRepository
{
    Appointment Add(Appointment appointment);
    List<Appointment> GetAll();
}
