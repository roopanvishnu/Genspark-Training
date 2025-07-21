using FirstAPI.Models;
using FirstAPI.Repository.Interface;
using FirstAPI.Services.Interfaces;

namespace FirstAPI.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentRepository _appointmentRepository;

    public AppointmentService(IAppointmentRepository repo)
    {
        _appointmentRepository = repo;
    }

    public IEnumerable<Appointment> GetAppointments()
    {
        return _appointmentRepository.GetAllAppointments();
    }

    public Appointment? GetAppointmentById(int id)
    {
        return _appointmentRepository.GetAppointmentById(id);
    }

    public void CreateAppointment(Appointment appointment)
    {
        _appointmentRepository.CreateAppointment(appointment);
    }

    public void DeleteAppointment(int id)
    {
        _appointmentRepository.DeleteAppointment(id);
    }

    
}

