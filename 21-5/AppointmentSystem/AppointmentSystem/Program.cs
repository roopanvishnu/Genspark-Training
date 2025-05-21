using AppointmentSystem.Models;
using AppointmentSystem.Repositories;
using AppointmentSystem.Services;

class Program
{
    static void Main(string[] args)
    {
        var repository = new InMemoryAppointmentRepository();
        var service = new AppointmentService(repository);

        while (true)
        {
            Console.WriteLine("\n--- Cardiologist Appointment Manager ---");
            Console.WriteLine("1. Add Appointment");
            Console.WriteLine("2. Search Appointments");
            Console.WriteLine("0. Exit");
            Console.Write("Choose: ");
            var input = Console.ReadLine();
            Action action = input switch
            {
                "1" => () => AddAppointment(service),
                "2" => () => SearchAppointments(service),
                "0" => () => Environment.Exit(0), 
                _ => () => Console.WriteLine("Invalid option")
            };

            action();
        }
    }

    static void AddAppointment(AppointmentService service)
    {
        Console.Write("Patient Name: ");
        string name = Console.ReadLine();

        Console.Write("Patient Age: ");
        int age = int.Parse(Console.ReadLine());

        Console.Write("Appointment Date/Time (yyyy-MM-dd HH:mm): ");
        DateTime dt = DateTime.Parse(Console.ReadLine());

        Console.Write("Reason for Visit: ");
        string reason = Console.ReadLine();

        var appt = new Appointment
        {
            PatientName = name,
            PatientAge = age,
            AppointmentDate = dt,
            Reason = reason
        };

        var added = service.AddAppointment(appt);
        Console.WriteLine($"Appointment added successfully! ID: {added.Id}");
    }

    static void SearchAppointments(AppointmentService service)
    {
        var searchModel = new AppointmentSearchModel();

        Console.Write("Patient Name (optional): ");
        string name = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(name))
            searchModel.PatientName = name;

        Console.Write("Appointment Date (yyyy-MM-dd, optional): ");
        string dateStr = Console.ReadLine();
        if (DateTime.TryParse(dateStr, out DateTime date))
            searchModel.AppointmentDate = date;

        Console.Write("Age Range Min (optional): ");
        string minAgeStr = Console.ReadLine();

        Console.Write("Age Range Max (optional): ");
        string maxAgeStr = Console.ReadLine();

        if (int.TryParse(minAgeStr, out int min) && int.TryParse(maxAgeStr, out int max))
            searchModel.AgeRange = (min, max);

        var results = service.SearchAppointments(searchModel);
        if (results.Count == 0)
        {
            Console.WriteLine("No matching appointments found.");
        }
        else
        {
            Console.WriteLine("Matching Appointments:");
            results.ForEach(a => Console.WriteLine(a));
        }
    }
}
