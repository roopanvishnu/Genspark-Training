using System;
using System.Security.Claims;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace FirstAPI.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IRepository<string, Appointment> _appointmentRepository;

    public AppointmentService(IRepository<string, Appointment> appointmentRepository)
    {
        _appointmentRepository = appointmentRepository;

    }
    public async Task<Appointment> Add(AppointmentAddRequestDTO addRequestDTO)
    {
        try
        {
            Appointment app = new Appointment
            {
                PatientId = addRequestDTO.PatientId,
                DoctorId = addRequestDTO.DoctorId,
                AppointmemtDateTime = addRequestDTO.AppointmentDateTime,
                Status = "Created",
                AppointmentNumber = addRequestDTO.DoctorId + "-" + addRequestDTO.PatientId + "-" + addRequestDTO.AppointmentDateTime.ToString("s")
            };
            app = await _appointmentRepository.Add(app);
            return app;
        }
        catch
        {
            throw;
        }
    }

    public async Task Cancel(string id)
    {
        Appointment? app = await Get(id);
        if (app == null) throw new Exception("Appointment not found");
        app.Status = "Cancelled";
        await _appointmentRepository.Update(app.AppointmentNumber, app);
    }

    public async Task<Appointment> Get(string id)
    {
        Appointment? app = await _appointmentRepository.Get(id);
        if (app == null) throw new Exception("Appointment not found");
        return app;
    }

    public async Task<ICollection<Appointment>> GetAll()
    {
        var apps = await _appointmentRepository.GetAll();
        if (apps == null) throw new Exception("No appointments found!");
        return apps.ToList();
    }
}
