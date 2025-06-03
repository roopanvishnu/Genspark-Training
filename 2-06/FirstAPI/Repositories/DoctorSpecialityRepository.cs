using System;
using System.Security.Cryptography;
using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories;

public class DoctorSpecialityRepository : Repository<int, DoctorSpeciality>
{
    public DoctorSpecialityRepository(ClinicContext _clinicContext) : base(_clinicContext) { }
    public override async Task<DoctorSpeciality> Get(int key)
    {
        var ds = await _clinicContext.doctorSpecialities.SingleOrDefaultAsync(ds => ds.SerialNumber == key);
        if (ds == null)
        {
            throw new Exception("No data found");
        }
        return ds;
    }

    public override async Task<IEnumerable<DoctorSpeciality>> GetAll()
    {
        var ds = _clinicContext.doctorSpecialities;
        if (ds.Count() == 0)
        {
            throw new Exception("No data found");
        }
        return (await ds.ToListAsync());
    }
}
