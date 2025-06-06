using TaskManagement.API.DTOs.Request.Auth;
using TaskManagement.API.DTOs.Response.Auth;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.Models.Entities;
using TaskManagement.API.Repositories.Interfaces;
using TaskManagement.API.Repositories.Interfaces.UnitOfWork;

namespace TaskManagement.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<ApiResponseDto<LoginResponseDto>> LoginAsync(LoginRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
        if (user == null || user.PasswordHash != request.password)
        {
            return ApiResponseDto<LoginResponseDto>.ErrorResponse("Invalid credentials");
        }

        var accessToken = await _jwtService.GenerateAccessTokenAsync(user.Id, user.Email, user.Role.ToString());
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync();
        // store refresh token (simplified)
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            User = new UserProfileResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                PhoneNumber = user.PhoneNumber,
                ProfilePicture = user.ProfilePicture,
                IsActive = user.IsActive,
                LastLoginAt = user.LoginAt,
                CreatedAt = user.CreatedAt
            }
        };

        return ApiResponseDto<LoginResponseDto>.SuccessResponse(response);
    }

    public async Task<ApiResponseDto<RefreshTokenResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return ApiResponseDto<RefreshTokenResponseDto>.ErrorResponse("Invalid refresh token");
        }

        var accessToken = await _jwtService.GenerateAccessTokenAsync(user.Id, user.Email, user.Role.ToString());
        var refreshToken = await _jwtService.GenerateRefreshTokenAsync();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var response = new RefreshTokenResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        return ApiResponseDto<RefreshTokenResponseDto>.SuccessResponse(response);
    }

    public async Task<ApiResponseDto<string>> LogoutAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return ApiResponseDto<string>.ErrorResponse("Invalid user id");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponseDto<string>.ErrorResponse("User not found");
        }

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<string>.SuccessResponse("Logged out");
    }

    public async Task<ApiResponseDto<UserProfileResponseDto>> GetCurrentUserAsync(string userId)
    {
        if (!Guid.TryParse(userId, out var id))
        {
            return ApiResponseDto<UserProfileResponseDto>.ErrorResponse("Invalid user id");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(id);
        if (user == null)
        {
            return ApiResponseDto<UserProfileResponseDto>.ErrorResponse("User not found");
        }

        var dto = new UserProfileResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            IsActive = user.IsActive,
            LastLoginAt = user.LoginAt,
            CreatedAt = user.CreatedAt
        };

        return ApiResponseDto<UserProfileResponseDto>.SuccessResponse(dto);
    }

    public async Task<ApiResponseDto<UserProfileResponseDto>> RegisterAsync(RegisterRequestDto request)
    {
        var exists = await _unitOfWork.Users.EmailExistAsync(request.Email);
        if (exists)
        {
            return ApiResponseDto<UserProfileResponseDto>.ErrorResponse("Email already exists");
        }

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = request.Role,
            PhoneNumber = request.PhoneNumber,
            CreatedBy = request.Email
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        var dto = new UserProfileResponseDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            PhoneNumber = user.PhoneNumber,
            ProfilePicture = user.ProfilePicture,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };

        return ApiResponseDto<UserProfileResponseDto>.SuccessResponse(dto);
    }
}
