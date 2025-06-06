using TaskManagement.API.DTOs.Pagination;
using TaskManagement.API.DTOs.Request.User;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.User;
using TaskManagement.API.Models.Entities;
using TaskManagement.API.Repositories.Interfaces;
using TaskManagement.API.Repositories.Interfaces.UnitOfWork;

namespace TaskManagement.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResponseDto<UserResponseDto>> GetUsersAsync(PaginationParameters pagination)
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var total = users.Count();
        var items = users.Skip(pagination.Skip).Take(pagination.PageSize)
            .Select(u => MapToDto(u)).ToList();

        return new PaginatedResponseDto<UserResponseDto>
        {
            Success = true,
            Message = "Users fetched",
            Data = items,
            Pagination = new PaginationMetadata
            {
                TotalCount = total,
                Page = pagination.Page,
                PageSize = pagination.PageSize,
                TotalPages = (int)Math.Ceiling((double)total / pagination.PageSize)
            }
        };
    }

    public async Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        return user == null
            ? ApiResponseDto<UserResponseDto>.ErrorResponse("User not found")
            : ApiResponseDto<UserResponseDto>.SuccessResponse(MapToDto(user));
    }

    public async Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request, string createdBy)
    {
        var exists = await _unitOfWork.Users.EmailExistAsync(request.Email);
        if (exists)
            return ApiResponseDto<UserResponseDto>.ErrorResponse("Email already exists");

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = request.Password,
            Role = request.Role,
            PhoneNumber = request.PhoneNumber,
            CreatedBy = createdBy
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<UserResponseDto>.SuccessResponse(MapToDto(user), "User created");
    }

    public async Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(Guid userId, UpdateUserRequestDto request, string updatedBy)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return ApiResponseDto<UserResponseDto>.ErrorResponse("User not found");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.PasswordHash = request.Password;
        user.Role = request.Role;
        user.PhoneNumber = request.PhoneNumber;
        user.IsActive = request.IsActive;
        user.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<UserResponseDto>.SuccessResponse(MapToDto(user), "User updated");
    }

    public async Task<ApiResponseDto<string>> DeleteUserAsync(Guid userId, string deletedBy)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
            return ApiResponseDto<string>.ErrorResponse("User not found");

        await _unitOfWork.Users.DeleteAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return ApiResponseDto<string>.SuccessResponse("User deleted");
    }

    public async Task<ApiResponseDto<List<UserResponseDto>>> GetTeamMembersAsync()
    {
        var users = await _unitOfWork.Users.GetUserByRoleAsync(Models.Enums.UserRole.TeamMember);
        var list = users.Select(MapToDto).ToList();
        return ApiResponseDto<List<UserResponseDto>>.SuccessResponse(list);
    }

    private static UserResponseDto MapToDto(User user) => new UserResponseDto
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        Role = user.Role,
        PhoneNumber = user.PhoneNumber,
        ProfilePictureUrl = user.ProfilePicture,
        IsActive = user.IsActive,
        LastLoginAt = user.LoginAt ?? DateTime.MinValue,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
    };
}
