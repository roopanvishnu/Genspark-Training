using System;
using System.Threading.Tasks;
using Notify.Interfaces;
using Notify.Models;
using Notify.Models.DTOs;
using Notify.Repositories;

namespace Notify.Services;

public class UserService
{
    private readonly IRepo<int, User> _userRepo;
    private readonly HashingService _hashingService;
    public UserService(IRepo<int, User> repo, HashingService hashingService)
    {
        _userRepo = repo;
        _hashingService = hashingService;
    }
    public async Task<User> AddUser(UserAddRequestDTO dto)
    {
        User user = new User
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role
        };
        HashDTO hashedData = new HashDTO { Data = dto.Password };
        hashedData = _hashingService.HashData(hashedData);
        user.Password = hashedData.HashedData;
        user.HashKey = hashedData.HashKey;

        user = await _userRepo.Add(user);
        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var users = await _userRepo.GetAll();
        var user = users.FirstOrDefault(u => u.Email == email);
        if (user == null) throw new Exception("No user found");
        return user;
    }
    public async Task<ICollection<User>> GetAll()
    {
        var users = await _userRepo.GetAll();
        if (users == null || users.Count()==0) throw new Exception("No user found");
        return users;
    }
}
