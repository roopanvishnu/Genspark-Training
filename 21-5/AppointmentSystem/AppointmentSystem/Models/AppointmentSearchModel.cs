namespace AppointmentSystem.Models;

public class AppointmentSearchModel
{
    public string? PatientName { get; set; }
    public DateTime? AppointmentDate { get; set; }
    public (int Min, int Max)? AgeRange { get; set; }
}
