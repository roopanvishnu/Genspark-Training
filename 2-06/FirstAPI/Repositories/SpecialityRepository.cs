using System;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories;

public class SpecialityRepository : Repository<int, Speciality>
{
    public SpecialityRepository(ClinicContext _clinicContext) : base(_clinicContext) { }
    public override async Task<Speciality> Get(int key)
    {
        var sp = await _clinicContext.specialities.FindAsync(key);
        if (sp == null)
        {
            throw new Exception("No data found");
        }
        return sp;
    }

    public override async Task<IEnumerable<Speciality>> GetAll()
    {
        var sps = _clinicContext.specialities;
        if (sps.Count() == 0)
        {
            throw new Exception("No data found");
        }
        return (await sps.ToListAsync());
    }
}
