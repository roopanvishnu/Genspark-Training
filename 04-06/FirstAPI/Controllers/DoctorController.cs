using FirstAPI.Interfaces;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FirstAPI.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        public DoctorController(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }
        [HttpPost]
        public async Task<ActionResult<Doctor>> AddDoctor([FromBody] DoctorAddRequestDTO doctor)
        {
            try
            {
                Doctor doc = await _doctorService.AddDoctor(doctor);
                if (doc != null)
                    return Created("", doc);
                return BadRequest("Unable to create Doctor");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<DoctorBySpecialityResponseDTO>>> GetDoctorBySpeciality(string speciality)
        {
            var result = await _doctorService.GetDoctorsBySpeciality(speciality);
            if (result == null || result.Count() == 0) return NotFound("No doctors found");
            return Ok(result);
        }
    }
    // public class DoctorController : ControllerBase
    // {
    //     static List<Doctor> list = new List<Doctor>
    //     {
    //         new Doctor{ Id = 1, Name ="Hex"},
    //         new Doctor{ Id = 2, Name ="Hept"}
    //     };


    //     [HttpGet]
    //     public ActionResult<IEnumerable<Doctor>> GetDoctors()
    //     {
    //         return Ok(list);
    //     }

    //     [HttpPost]
    //     public ActionResult<Doctor> PostDoctor([FromBody] Doctor doctor)
    //     {
    //         list.Add(doctor);
    //         return Created("", doctor);
    //     }

    //     [HttpDelete]
    //     public ActionResult DeleteDoctor(int id)
    //     {
    //         Doctor? del = list.Where(x => x.Id == id).FirstOrDefault();
    //         if (del != null)
    //         {
    //             list.Remove(del);
    //             return Ok(del);
    //         }
    //         return NotFound("No data Found");
    //     }

    //     [HttpPut]
    //     public ActionResult<Doctor> PutDoctor([FromBody] Doctor doctor)
    //     {
    //         Doctor? doc = list.Where(d => d.Id == doctor.Id).FirstOrDefault();
    //         if(doc == null) 
    //             return NotFound("No data Found");

    //         // int index = list.IndexOf(doc);
    //         // list.Remove(doc);
    //         // list.Insert(index, doctor);
    //         doc.Name = doctor.Name;
    //         return Ok(doctor);

    //     }
    // }
}