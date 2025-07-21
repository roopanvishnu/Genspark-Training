using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Repositories;
using FirstApi.Services;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly AppointmentService _appointmentService;

    public AppointmentController(AppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Appointment>> GetAppointments()
    {
        var appointments = _appointmentService.GetAppointments();
        return Ok(appointments);
    }

    [HttpPost]
    public ActionResult<Appointment> PostAppointment([FromBody] Appointment appointment)
    {
        var createdAppointment = _appointmentService.AddAppointment(appointment);
        return Created("", createdAppointment);
    }

    [HttpPut("{id}")]
    public ActionResult<Appointment> UpdateAppointment(int id, [FromBody] Appointment updatedAppointment)
    {
        var appointment = _appointmentService.UpdateAppointment(id, updatedAppointment);
        if (appointment == null)
        {
            return NotFound("Appointment not found");
        }
        return Ok(appointment);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteAppointment(int id)
    {
        var deleted = _appointmentService.DeleteAppointment(id);
        if (!deleted)
        {
            return NotFound("Appointment not found");
        }
        return NoContent();
    }
}