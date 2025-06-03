using System.Threading.Tasks;
using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;
        public PatientController(
                                    IPatientService patientService,
                                    ILogger<PatientController> logger
                                )
        {
            _patientService = patientService;
            _logger = logger;
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            try
            {
                var patients = await _patientService.GetPatients();
                return Ok(patients);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            Patient? p = await _patientService.GetPatientById(id);
            if (p == null)
                return NotFound("No data found");
            return Ok(p);
        }
        


        [HttpPost]
        public async Task<ActionResult<Patient>> AddPatient(PatientAddRequestDTO patient)
        {
            try
            {
                var p = await _patientService.AddPatient(patient);
                return Created("", p);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}