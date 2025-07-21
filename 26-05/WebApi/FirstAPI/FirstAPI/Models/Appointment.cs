/*
namespace FirstAPI.Models;

public class Appointment
{
    public int Id { set; get; }
    public int DoctorId { set; get; }
    public int PatientId { set; get; }
    public DateTime Date { set; get; }
}
*/

using System.ComponentModel.DataAnnotations;

namespace FirstAPI.Models;

public class Appointment
{
    public int Id { set; get; }
    public int DoctorId { set; get; }
    public string? DoctorName { set; get; }
    public string? PatientName { set; get; }
    public int PatientId { set; get; }  
    public DateTime Date { set; get; }
}