using BCrypt.Net;
using ChienVHShopOnline.DTOs;
using ChienVHShopOnline.Models;
using ChienVHShopOnline.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ChienVHShopOnline.Services;

public class UserService
{
    private UserRepo _userRepo;
    public UserService(UserRepo UserRepo)
    {
        _userRepo = UserRepo;
    }

    public async Task<List<User>> GetAll()
    {
        return (await _userRepo.GetAll()).ToList();
    }

    public async Task<User> Get(int id)
    {
        return (await _userRepo.Get(id));
    }

    public async Task<User> Create([FromBody] UserDTO userDTO)
    {
        var User = new User
        {
            Username = userDTO.Username,
            Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDTO.Password)
        };
        User = await _userRepo.Add(User);
        return User;
    }
    public async Task<User> Edit(int id, [FromBody] UserDTO userDTO)
    {
        User user = await _userRepo.Get(id);
        user.Username = userDTO.Username;
        user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userDTO.Password);


        user = await _userRepo.Update(id, user);
        return user;
    }
    public async Task<User> Delete(int id)
    {
        User user = await _userRepo.Delete(id);
        return user;
    }
    
}