using System.Security.Claims;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using FirstAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        public AppointmentController(IAppointmentService appointmentService, IPatientService patientService, IDoctorService doctorService)
        {
            _appointmentService = appointmentService;
            _patientService = patientService;
            _doctorService = doctorService;
        }

        [HttpPost]
        public async Task<ActionResult<Appointment>> Add(AppointmentAddRequestDTO addRequestDTO)
        {
            try
            {
                Appointment app = await _appointmentService.Add(addRequestDTO);
                return Created("", app);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<Appointment>>> GetAll()
        {
            try
            {
                var apps = await _appointmentService.GetAll();
                var user = User;
                if (user == null) return Unauthorized("Unauthorized Access");
                var role = user.FindFirst(ClaimTypes.Role)?.Value;
                var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                System.Console.WriteLine(role+" "+email);
                if (role == "Patient")
                {
                    var patients = await _patientService.GetPatients();
                    var patient = patients.FirstOrDefault(p => p.Email == email);
                    if (patient == null) return NotFound("Patient not found");
                    apps = apps.Where(a => a.PatientId == patient.Id).ToList();
                }
                if (apps == null) return NotFound("No appointments found!");
                return Ok(apps.ToList());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> Get(string id)
        {
            try
            {
                var app = await _appointmentService.Get(id);
                if (app == null) return NotFound("No such appointment found");
                return Ok(app);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Policy = "DoctorWithMin3yoe")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<Appointment>> Cancel(string id)
        {
            try
            {
                Appointment? app = await _appointmentService.Get(id);
                if (app == null) return NotFound("Appointment not found");

                var user = User;
                var role = user.FindFirst(ClaimTypes.Role)?.Value;
                var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                System.Console.WriteLine(role +" "+email);
                var doc = await _doctorService.GetDoctorByEmail(email!);
                if (doc == null) return Unauthorized("Unauthorized access");
                if (app.DoctorId != doc.Id) return Unauthorized("UnAuthorized Delete Action");
                await _appointmentService.Cancel(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }
    }
}
