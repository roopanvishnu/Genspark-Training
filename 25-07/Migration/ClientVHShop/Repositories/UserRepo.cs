using ChienVHShopOnline.Contexts;
using ChienVHShopOnline.Models;
using Microsoft.EntityFrameworkCore;

namespace ChienVHShopOnline.Repositories;

public class UserRepo : Repo<int, User>
{
    public UserRepo(ChienVHShopDBEntities context) : base(context)
    {
    }

    public override async Task<User> Get(int id)
    {
        return await _context.Users.FindAsync(id) ?? throw new Exception("User not found");
    }

    public override async Task<ICollection<User>> GetAll()
    {
        return await _context.Users.ToListAsync() ?? throw new Exception("No Users found");
    }
}