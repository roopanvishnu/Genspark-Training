using AppointmentSystem.Models;
using AppointmentSystem.Repositories;

namespace AppointmentSystem.Services;

public class AppointmentService
{
    private readonly IAppointmentRepository _repository;

    public AppointmentService(IAppointmentRepository repository)
    {
        _repository = repository;
    }

    public Appointment AddAppointment(Appointment appointment)
    {
        try
        {
            return _repository.Add(appointment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding appointment: {ex.Message}");
            throw;
        }
    }

    public List<Appointment> SearchAppointments(AppointmentSearchModel search)
    {
        try
        {
            var results = _repository.GetAll();

            if (!string.IsNullOrWhiteSpace(search.PatientName))
                results = SearchByName(results, search.PatientName);

            if (search.AppointmentDate.HasValue)
                results = SearchByDate(results, search.AppointmentDate.Value);

            if (search.AgeRange.HasValue)
                results = SearchByAge(results, search.AgeRange.Value.Min, search.AgeRange.Value.Max);

            return results;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error searching appointments: {ex.Message}");
            return new List<Appointment>();
        }
    }

    private List<Appointment> SearchByName(List<Appointment> list, string name)
    {
        return list.Where(a => a.PatientName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    private List<Appointment> SearchByDate(List<Appointment> list, DateTime date)
    {
        return list.Where(a => a.AppointmentDate.Date == date.Date).ToList();
    }

    private List<Appointment> SearchByAge(List<Appointment> list, int min, int max)
    {
        return list.Where(a => a.PatientAge >= min && a.PatientAge <= max).ToList();
    }
}
