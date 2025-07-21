using FirstAPI.Models;
using FirstAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentController(IAppointmentService appointmentService)
    {
        _service = appointmentService;
    }

    [HttpGet]
    //public OkObjectResult GetAppointments()=> Ok(_service.GetAppointments());
    public ActionResult GetAppointments()
    {
        return Ok(_service.GetAppointments());
    }

    [HttpGet("{id}")]
    public ActionResult GetAppointment(int id)
    {
        return Ok(_service.GetAppointmentById(id));
    }

    [HttpPost]
    public ActionResult CreateAppointment([FromBody] Appointment appointment)
    {
        _service.CreateAppointment(appointment);
        return Ok("Appointment created");
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteAppointment(int id)
    {
        _service.DeleteAppointment(id);
        return Ok("Appointment deleted");
    }
}