using FirstAPI.Models;

namespace  FirstAPI.Interfaces;


public interface IAppointmentService
{
    Task<Appointmnet> GetApppointmentById(string appointmentNumber);
    Task<IEnumerable<Appointmnet>> GetAllAppointments();
    Task<Appointmnet> AddNewAppointment(Appointmnet appointment);
    Task<Appointmnet> UpdateAppointment(Appointmnet appointment,string appointmentNumber);
    Task<Appointmnet> DeleteAppointment(string appointmentNumber);
}