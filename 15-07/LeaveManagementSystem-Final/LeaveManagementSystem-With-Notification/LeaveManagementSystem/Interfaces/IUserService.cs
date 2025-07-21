using LeaveManagementSystem.Models.DTOs;

namespace LeaveManagementSystem.Interfaces
{
    public interface IUserService
    {
        Task<(IEnumerable<UserDto> Users, int TotalCount)> GetAllUsers(int pageNumber, int pageSize, string searchTerm, string role, string sortBy, string sortOrder);
        Task<UserDto> GetUserById(Guid id);
        Task<UserDto> CreateUser(CreateUserDto createUserDto);
        Task<UserDto> UpdateUser(Guid id, UpdateUserDto updateUserDto);
        Task<UserDto> DeleteUser(Guid id);
        Task<bool> ChangePasswordAsync(ChangePasswordDto dto);
        Task<UserDetailDto> GetUserDetailById(Guid id, int skip, int take);

    }
}
