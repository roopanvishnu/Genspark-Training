using FirstApi.Models;
using Microsoft.AspNetCore.Mvc;
using FirstApi.Services;


[ApiController]
[Route("api/[controller]")]
public class PatientController : ControllerBase
{
    private readonly PatientService _patientService;

    public PatientController(PatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Patient>> GetPatients()
    {
        var patients = _patientService.GetPatients();
        return Ok(patients);
    }

    [HttpPost]
    public ActionResult<Patient> PostPatient([FromBody] Patient patient)
    {
        var createdPatient = _patientService.AddPatient(patient);
        return Created("", createdPatient);
    }

    [HttpPut("{id}")]
    public ActionResult<Patient> UpdatePatient(int id, [FromBody] Patient updatedPatient)
    {
        var patient = _patientService.UpdatePatient(id, updatedPatient);
        if (patient == null)
        {
            return NotFound("Patient not found");
        }
        return Ok(patient);
    }

    [HttpDelete("{id}")]
    public ActionResult DeletePatient(int id)
    {
        var deleted = _patientService.DeletePatient(id);
        if (!deleted)
        {
            return NotFound("Patient not found");
        }
        return NoContent();
    }


}

