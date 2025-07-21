using FirstAPI.Models;
using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("/api/[controller]")]
public class DoctorControllers : ControllerBase
{
    static List<Doctors> Doctors = new List<Doctors>
    {
        new Doctors { Id = 1, Name = "Some Doctor named 1" },
        new Doctors { Id = 2, Name = "Some Doctor named 2" },
    };

    [HttpGet]
    public ActionResult <IEnumerable<Doctors>>GetAllDoctors()
    {
        return Ok(Doctors);
    }

    [HttpPost]
    public ActionResult AddDoctor(Doctors doctor)
    {
        Doctors.Add(doctor);
        return Created("",doctor);
    }

    [HttpDelete("{id}")]
    public ActionResult DeleteDoctor(int id)
    {
        var doctor = Doctors.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            return NotFound("Doctor not found");
        }
        Doctors.Remove(doctor);
        return Ok("Deleted");
    }

    [HttpPut]
    public ActionResult UpdateDoctor(int id,Doctors UpdatedDoctor)
    {
        var ExistingDoctor = Doctors.FirstOrDefault(d => d.Id == id);
        if (ExistingDoctor == null)
        {
            return NotFound("Doctor not found");
        }
        ExistingDoctor.Name = UpdatedDoctor.Name;
        return Ok("Updated");
    }
}