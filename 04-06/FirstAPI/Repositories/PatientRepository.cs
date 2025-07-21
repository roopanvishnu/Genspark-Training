using System;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories;

public class PatientRepository : Repository<int, Patient>
{
    public PatientRepository(ClinicContext clinicContext) : base(clinicContext)
    {
    }

    public override async Task<Patient> Get(int key)
    {
        var patient = await _clinicContext.patients.SingleOrDefaultAsync(p => p.Id == key);
        if (patient== null) throw new Exception("No data found");
        return patient;
    }

    public override async Task<IEnumerable<Patient>> GetAll()
    {
        var patients = _clinicContext.patients;
        if (patients.Count() == 0)
            throw new Exception("No data found");
        return await patients.ToListAsync();
    }
}
