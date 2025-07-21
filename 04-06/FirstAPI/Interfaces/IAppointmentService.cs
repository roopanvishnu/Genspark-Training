using System;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IAppointmentService
{
    Task<Appointment> Add(AppointmentAddRequestDTO appointment);
    Task<ICollection<Appointment>> GetAll();

    Task Cancel(string id);
    Task<Appointment> Get(string id);

}
