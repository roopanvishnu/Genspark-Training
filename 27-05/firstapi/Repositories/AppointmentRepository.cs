namespace FirstApi.Repositories;

using FirstApi.Models;
public class AppointmentRepository : IAppointmentRepository
{
    private static List<Appointment> _appointments = new List<Appointment>();

    public IEnumerable<Appointment> GetAppointments()
    {
        return _appointments;
    }

    public Appointment AddAppointment(Appointment appointment)
    {
        appointment.Id = _appointments.Count > 0 ? _appointments.Max(a => a.Id) + 1 : 1;
        _appointments.Add(appointment);
        return appointment;
    }

    public Appointment UpdateAppointment(int id, Appointment updatedAppointment)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            appointment.PatientId = updatedAppointment.PatientId;
            appointment.DoctorId = updatedAppointment.DoctorId;
            appointment.Status = updatedAppointment.Status;
            return appointment;
        }
        return null;
    }

    public bool DeleteAppointment(int id)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == id);
        if (appointment != null)
        {
            _appointments.Remove(appointment);
            return true;
        }
        return false;
    }
}