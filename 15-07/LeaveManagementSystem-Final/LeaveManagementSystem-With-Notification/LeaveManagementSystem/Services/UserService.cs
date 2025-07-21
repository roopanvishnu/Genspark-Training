using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Helpers;
using LeaveManagementSystem.Exceptions;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILeaveBalanceService _leaveBalanceService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public UserService(
            IRepository<Guid, User> userRepository, 
            IAuditLogService auditLogService, 
            ICurrentUserService currentUserService, 
            ILeaveBalanceService leaveBalanceService,
            IMapper mapper,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _leaveBalanceService = leaveBalanceService;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<(IEnumerable<UserDto> Users, int TotalCount)> GetAllUsers(
                int pageNumber, int pageSize, string searchTerm, string role, string sortBy, string sortOrder)
        {
            try
            {
                var users = await _userRepository.GetAll();
                var userDtos = _mapper.Map<List<UserDto>>(users);

                // Search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    userDtos = userDtos.Where(u =>
                        (!string.IsNullOrEmpty(u.Username) && u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    ).ToList();
                }

                // Filter
                if (!string.IsNullOrWhiteSpace(role))
                {
                    userDtos = userDtos.Where(u => u.Role.Equals(role, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                // Total count BEFORE pagination
                int totalCount = userDtos.Count;

                // Sort
                userDtos = sortBy?.ToLower() switch
                {
                    "name" => (sortOrder == "desc") ? userDtos.OrderByDescending(u => u.Username).ToList() : userDtos.OrderBy(u => u.Username).ToList(),
                    "email" => (sortOrder == "desc") ? userDtos.OrderByDescending(u => u.Email).ToList() : userDtos.OrderBy(u => u.Email).ToList(),
                    _ => (sortOrder == "desc") ? userDtos.OrderByDescending(u => u.CreatedAt).ToList() : userDtos.OrderBy(u => u.CreatedAt).ToList(),
                };

                // Pagination
                userDtos = userDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return (userDtos, totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve users.", ex);
            }
        }


        public async Task<UserDto> GetUserById(Guid id)
        {
            try
            {
                var user = await _userRepository.Get(id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");
                return _mapper.Map<UserDto>(user);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve user with id {id}.", ex);
            }
        }

        public async Task<UserDetailDto> GetUserDetailById(Guid id, int page = 1, int pageSize = 10)
        {
            try
            {
                var user = await _userRepository.Get(id);
                if (user == null)
                    throw new KeyNotFoundException("User not found.");

                var leaveRequests = user.LeaveRequests?
                    .OrderByDescending(lr => lr.CreatedAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(lr => new LeaveRequestResponseDto
                    {
                        Id = lr.Id,
                        UserId = lr.UserId,
                        UserName = user.Username,
                        LeaveTypeId = lr.LeaveTypeId,
                        LeaveTypeName = lr.LeaveType?.Name,
                        StartDate = lr.StartDate,
                        EndDate = lr.EndDate,
                        Reason = lr.Reason,
                        Status = lr.Status,
                        ReviewedById = lr.ReviewedById,
                        ReviewedByName = lr.ReviewedBy?.Username,
                        CreatedAt = lr.CreatedAt,
                        UpdatedAt = lr.UpdatedAt ?? lr.CreatedAt

                    }).ToList();

                var userDetailDto = new UserDetailDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role,
                    Gender = user.Gender,
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt,
                    CreatedAt = user.CreatedAt,
                    CreatedBy = user.CreatedBy,
                    IsDeleted = user.IsDeleted,
                    DeletedAt = user.DeletedAt,

                    LeaveBalances = user.LeaveBalances?.Select(lb => new LeaveBalanceResponseDto
                    {
                        Id = lb.Id,
                        UserId = lb.UserId,
                        LeaveTypeId = lb.LeaveTypeId,
                        LeaveTypeName = lb.LeaveType?.Name,
                        TotalLeaves = lb.TotalLeaves,
                        UsedLeaves = lb.UsedLeaves,
                        RemainingLeaves = lb.RemainingLeaves
                    }).ToList(),

                    LeaveRequests = leaveRequests
                };

                return userDetailDto;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve user details for id {id}.", ex);
            }
        }



        // public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
        // {
        //     try
        //     {
        //         var user = _mapper.Map<User>(createUserDto);
        //         user.Id = Guid.NewGuid();

        //         user.PasswordHash = PasswordHelper.HashPassword(createUserDto.Password);
        //         user.CreatedAt = DateTime.UtcNow;
        //         user.CreatedBy = _currentUserService.GetUserId();

        //         var createdUser = await _userRepository.Add(user);

        //         await _leaveBalanceService.InitializeLeaveBalancesForUserAsync(createdUser.Id);

        //         await _auditLogService.LogAsync(_currentUserService.GetUserId(), "INSERTED", "USER", createdUser.Id);

        //         await _emailService.SendAsync(
        //                         user.Email,
        //                         "Welcome to the Leave Management System",
        //                         $@"Hi {user.Username},

        //             Welcome to the Leave Management System!

        //             Your account has been successfully created.

        //             👉 Login Credentials:
        //             Email: {user.Email}
        //             Password: {createUserDto.Password}

        //             ⚠️ Please log in and change your password immediately.

        //             Best regards,  
        //             HR Team"
        //         );

        //         return _mapper.Map<UserDto>(createdUser);
        //     }
        //     catch (DbUpdateException dbEx)
        //     {
        //         if (dbEx.InnerException != null &&
        //             dbEx.InnerException.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
        //         {
        //             throw new DuplicateKeyException("A user with the same unique field (like email or username) already exists.", dbEx);
        //         }
        //         throw;
        //     }
        //     catch (Exception ex)
        //     {
        //         throw new Exception("Failed to create user.", ex);
        //     }
        // }
        public async Task<UserDto> CreateUser(CreateUserDto createUserDto)
{
    try
    {
        var user = _mapper.Map<User>(createUserDto);
        user.Id = Guid.NewGuid();
        user.PasswordHash = PasswordHelper.HashPassword(createUserDto.Password);
        user.CreatedAt = DateTime.UtcNow;

        // Try to get current user ID (if token exists)
        Guid? createdBy = null;
        try
        {
            createdBy = _currentUserService.GetUserId();
        }
        catch
        {
            // No auth context; this is allowed during bootstrap
        }

        user.CreatedBy = createdBy;

        var createdUser = await _userRepository.Add(user);

        await _leaveBalanceService.InitializeLeaveBalancesForUserAsync(createdUser.Id);

        // Only log and notify if creator exists
        if (createdBy != null)
        {
            await _auditLogService.LogAsync(createdBy.Value, "INSERTED", "USER", createdUser.Id);
        }

        await _emailService.SendAsync(
            user.Email,
            "Welcome to the Leave Management System",
            $@"Hi {user.Username},

Welcome to the Leave Management System!

Your account has been successfully created.

👉 Login Credentials:
Email: {user.Email}
Password: {createUserDto.Password}

⚠️ Please log in and change your password immediately.

Best regards,  
HR Team"
        );

        return _mapper.Map<UserDto>(createdUser);
    }
    catch (DbUpdateException dbEx)
    {
        if (dbEx.InnerException != null &&
            dbEx.InnerException.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
        {
            throw new DuplicateKeyException("A user with the same email or username already exists.", dbEx);
        }
        throw;
    }
    catch (Exception ex)
    {
        throw new Exception("Failed to create user.", ex);
    }
}


        public async Task<UserDto> UpdateUser(Guid id, UpdateUserDto updateUserDto)
        {
            try
            {
                var existingUser = await _userRepository.Get(id);
                if (existingUser == null)
                    throw new KeyNotFoundException("User not found.");

                _mapper.Map(updateUserDto, existingUser);
                var updatedUser = await _userRepository.Update(id, existingUser);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "UPDATED",
                    "USER",
                    updatedUser.Id);

                return _mapper.Map<UserDto>(updatedUser);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                if (dbEx.InnerException != null &&
                    dbEx.InnerException.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    throw new DuplicateKeyException("A user with the same unique field (like email or username) already exists.", dbEx);
                }
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update user with id {id} {ex.Message}.", ex);
            }
        }

        public async Task<UserDto> DeleteUser(Guid id)
        {
            try
            {
                var existingUser = await _userRepository.Get(id);
                if (existingUser == null)
                    throw new KeyNotFoundException("User not found.");

                var deletedUser = await _userRepository.Delete(id);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(), 
                    "DELETED", 
                    "USER", 
                    deletedUser.Id
                );

                return _mapper.Map<UserDto>(deletedUser);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to delete user with id {id}.", ex);
            }
        }

        public async Task<bool> ChangePasswordAsync(ChangePasswordDto dto)
        {
            var user = (await _userRepository.GetAll())
                .FirstOrDefault(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            if (!PasswordHelper.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Current password is incorrect.");

            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ArgumentException("New password and confirm password do not match.");

            user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
            await _userRepository.Update(user.Id, user);

            await _auditLogService.LogAsync(
                _currentUserService.GetUserId(),
                "PASSWORD_CHANGED",
                "USER",
                user.Id
            );

            return true;
        }

    }
}
