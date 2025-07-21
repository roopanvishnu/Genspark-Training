using FirstAPI.Contexts;
using FirstAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FirstAPI.Repositories
{
    public class UserRepository : Repository<string, User>
    {
        public UserRepository(ClinicContext clinicContext) : base(clinicContext)
        {
        }

        public override async Task<User> Get(string key)
        {
            var user = await _clinicContext.users.SingleOrDefaultAsync(u => u.Username == key);
            if (user == null) throw new Exception("USer not found!");
            return user;
        }

        public override async Task<IEnumerable<User>> GetAll()
        {
            var users = await _clinicContext.users.ToListAsync();
            if (users == null || users.Count() == 0) throw new Exception("No users found");
            return users;
        }
    }
}