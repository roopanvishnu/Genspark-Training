using FirstAPI.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Contollers;
[ApiController]
[Route("/api/[controller]")]
public class PatientControllers : ControllerBase
{
    static List<Patients> Patients = new List<Patients>
    {
        new Patients { Id = 1, Name = "Ram", Age = 20 },
        new Patients { Id = 2, Name = "Sam", Age = 30 }
    };

    [HttpGet]
    public ActionResult<IEnumerable<Patients>> GetAllPatients()
    {
        return Ok(Patients);
    }

    [HttpPost]
    public ActionResult AddPatients(Patients patients)
    {
        Patients.Add(patients);
        return Ok("Added successfully");
    }

    [HttpDelete("{id}")]
    public ActionResult DeletePatients(int id)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound("Patient not found");
        }
        Patients.Remove(patient);
        return Ok("Deleted");
    }

    [HttpPut("{id}")]
    public ActionResult UpdatePatients(int id, Patients patients)
    {
        var patient = Patients.FirstOrDefault(p => p.Id == id);
        if (patient == null)
        {
            return NotFound("Patient not found");
        }

        patients.Name = patients.Name;
        patients.Age = patients.Age;
        return Ok("Updated");
    }
}