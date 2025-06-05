using System;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories;

public class DoctorRepository : Repository<int, Doctor>
{
    public DoctorRepository(ClinicContext clinicContext) : base(clinicContext)
    {
    }

    public override async Task<Doctor> Get(int key)
    {
        var doc = await _clinicContext.doctors.FindAsync(key);
        if (doc == null)
            throw new Exception("No data found");
        return doc;
    }

    public override async Task<IEnumerable<Doctor>> GetAll()
    {
        var docs = _clinicContext.doctors;
        if (docs.Count() == 0) throw new Exception("No data found");
        return (await docs.ToListAsync());
    }
}
