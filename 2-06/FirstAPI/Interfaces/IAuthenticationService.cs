
using System;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IAuthenticationService
{
    Task<UserLoginResponse> Login(UserLoginRequest req);
}
