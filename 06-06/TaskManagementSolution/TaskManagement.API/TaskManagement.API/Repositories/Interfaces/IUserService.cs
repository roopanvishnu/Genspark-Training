using TaskManagement.API.DTOs.Pagination;
using TaskManagement.API.DTOs.Request.User;
using TaskManagement.API.DTOs.Response.Common;
using TaskManagement.API.DTOs.Response.User;

namespace TaskManagement.API.Repositories.Interfaces;

public interface IUserService
{
    Task<PaginatedResponseDto<UserResponseDto>> GetUsersAsync(PaginationParameters pagination);
    Task<ApiResponseDto<UserResponseDto>> GetUserByIdAsync(Guid userId);
    Task<ApiResponseDto<UserResponseDto>> CreateUserAsync(CreateUserRequestDto request, string createdBy);
    Task<ApiResponseDto<UserResponseDto>> UpdateUserAsync(Guid userId, UpdateUserRequestDto request, string updatedBy);
    Task<ApiResponseDto<string>> DeleteUserAsync(Guid userId, string deletedBy);
    Task<ApiResponseDto<List<UserResponseDto>>> GetTeamMembersAsync();
}