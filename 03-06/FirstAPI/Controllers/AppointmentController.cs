using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FirstAPI.Services;
using FirstAPI.Interfaces;

[ApiController]
[Route("/api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpPut("cancel/{appointmentNumber}")]
    [Authorize(Policy = "ExperiencedDoctorOnly")]
    public async Task<IActionResult> CancelAppointment(string appointmentNumber)
    {
        try
        {
            var doctorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (doctorIdClaim == null || !int.TryParse(doctorIdClaim, out int doctorId))
                return Unauthorized("Invalid doctor info");

            var success = await _appointmentService.CancelAppointmentAsync(appointmentNumber, doctorId);

            if (!success)
                return BadRequest("Cannot cancel appointment");

            return Ok("Appointment canceled successfully");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    // New GET endpoint to fetch all dummy appointments
    [HttpGet]
    public IActionResult GetAppointments()
    {
        // Cast the IAppointmentService to InMemoryAppointmentService
        if (_appointmentService is InMemoryAppointmentService service)
        {
            return Ok(service.GetAllAppointments());
        }
        return NotFound("Appointment service not available.");
    }
}