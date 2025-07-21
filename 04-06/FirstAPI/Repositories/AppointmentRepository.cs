using System;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories;

public class AppointmentRepository : Repository<string, Appointment>
{
    public AppointmentRepository(ClinicContext clinicContext) : base(clinicContext)
    {
    }

    public override async Task<Appointment> Get(string key)
    {
        var app = await _clinicContext.appointments.SingleOrDefaultAsync(a => a.AppointmentNumber == key);
        if (app == null)
        {
            throw new Exception("No data found");
        }
        return app;
    }

    public override async Task<IEnumerable<Appointment>> GetAll()
    {
        var apps = _clinicContext.appointments;
        if (apps.Count() ==0)
            throw new Exception("No data found");
        return (await apps.ToListAsync());
    }
}
