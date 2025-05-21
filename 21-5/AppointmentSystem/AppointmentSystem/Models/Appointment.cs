namespace AppointmentSystem.Models;

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"ID: {Id}, Name: {PatientName}, Age: {PatientAge}, Date: {AppointmentDate}, Reason: {Reason}";
    }
}
