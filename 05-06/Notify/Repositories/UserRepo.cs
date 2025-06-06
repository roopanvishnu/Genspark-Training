using System;
using Microsoft.EntityFrameworkCore;
using Notify.Contexts;
using Notify.Models;

namespace Notify.Repositories;

public class UserRepo : Repo<int, User>
{
    public UserRepo(NotifyContext notifyContext) : base(notifyContext) { }


    public override async Task<User> Get(int id)
    {
        User? user = await _context.users.FindAsync(id);
        if (user == null) throw new Exception("User not found");
        return user;
    }

    public override async Task<ICollection<User>> GetAll()
    {
        var users = _context.users;
        if (users == null) throw new Exception("No users found");
        return await users.ToListAsync();
    }
}
