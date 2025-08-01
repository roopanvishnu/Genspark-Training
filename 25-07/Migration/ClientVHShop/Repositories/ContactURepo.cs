using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class ContactURepo : Repo<int, ContactU>
{
    public ContactURepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<ContactU> Get(int id)
    {
        return await _context.ContactUs.FindAsync(id) ?? throw new Exception("ContactU not found");
    }

    public override async Task<ICollection<ContactU>> GetAll()
    {
        return await _context.ContactUs.ToListAsync() ?? throw new Exception("No ContactUs found");
    }
}