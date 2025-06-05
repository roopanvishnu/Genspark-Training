
using System;
using FirstAPI.Models;
using FirstAPI.Models.DTOs;

namespace FirstAPI.Interfaces;

public interface IAuthenticationService
{
    Task<UserLoginResponse> Login(UserLoginRequest req);
    Task<GoogleLoginDTO> LoginWithGoogle(GoogleLoginDTO req);
}
