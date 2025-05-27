using Microsoft.AspNetCore.Mvc;
using FirstApi.Models;
using FirstApi.Repositories;
using FirstApi.Services;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly DoctorService _doctorService;

    public DoctorController(DoctorService doctorService)
    {
        _doctorService = doctorService;
    }

    [HttpGet]
    public ActionResult<IEnumerable<Doctor>> GetDoctors()
    {
        var doctors = _doctorService.GetDoctors();
        return Ok(doctors);
    }

    [HttpPost]
    public ActionResult<Doctor> PostDoctor([FromBody] Doctor doctor)
    {
        var createdDoctor = _doctorService.AddDoctor(doctor);
        return Created("",createdDoctor);
    }

    [HttpPut("{id}")]
    public ActionResult<Doctor> UpdateDoctor(int id, [FromBody] Doctor updatedDoctor)
    {
        var doctor = _doctorService.UpdateDoctor(id, updatedDoctor);
        if (doctor == null)
        {
            return NotFound("Doctor not found");
        }
        return Ok(doctor);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteDoctor(int id)
    {
        var deleted = _doctorService.DeleteDoctor(id);
        if (!deleted)
        {
            return NotFound("Doctor not found");
        }
        return NoContent();
    }
}