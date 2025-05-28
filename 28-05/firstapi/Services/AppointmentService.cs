using FirstAPI.Interfaces;
using FirstAPI.Models;

namespace FirstAPI.Services;

public class AppointmentService:IAppointmentService
{
    private readonly IRepository<string,Appointmnet> _appointmentRepository;
    private readonly IRepository<int ,Patient> _patientRepository;
    private readonly IRepository<int ,Doctor> _doctorRepository;
    public async Task<Appointmnet> GetApppointmentById(string appointmentNumber)
    {
        var appointment = await _appointmentRepository.Get(appointmentNumber);
        if (appointment == null)
        {
            throw new Exception("Appointment not found");
        }
        return appointment;
    }

    public async Task<IEnumerable<Appointmnet>> GetAllAppointments()
    {
        return await _appointmentRepository.GetAll();
    }

    public async Task<Appointmnet> AddNewAppointment(Appointmnet appointment)
    {
        var patient = await _patientRepository.Get(appointment.PatientId);
        if (patient == null)
        {
            throw new Exception("Patient not found");
        }
        var doctor = await _doctorRepository.Get(appointment.DoctorId);
        if (doctor == null)
        {
            throw new Exception("Doctor not found");
        }
        if(string.IsNullOrEmpty(appointment.AppointmnetNumber))
            appointment.AppointmnetNumber = Guid.NewGuid().ToString();
        
        appointment.Status = appointment.Status ?? "Scheduled";

        return await _appointmentRepository.Add(appointment);
    }

    public async Task<Appointmnet> UpdateAppointment(Appointmnet appointment, string appointmentNumber)
    {
        var existingAppointment = await _appointmentRepository.Get(appointmentNumber);
        if (existingAppointment == null)
        {
            throw new Exception("Appointment not found");
        }
        existingAppointment.AppointmnetDateTime = appointment.AppointmnetDateTime;
        existingAppointment.DoctorId = appointment.DoctorId;
        existingAppointment.PatientId = appointment.PatientId;
        existingAppointment.Status = appointment.Status;
        return await _appointmentRepository.Update(appointmentNumber, existingAppointment);
    }

    public async Task<Appointmnet> DeleteAppointment(string appointmentNumber)
    {
        var appointment = await _appointmentRepository.Get(appointmentNumber);
        if (appointment == null)
            throw new Exception("Appointment not found");

        return await _appointmentRepository.Delete(appointmentNumber);
    }
}