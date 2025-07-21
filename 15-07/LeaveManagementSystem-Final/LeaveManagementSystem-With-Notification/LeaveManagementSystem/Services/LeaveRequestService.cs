using AutoMapper;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using LeaveManagementSystem.Hubs;

namespace LeaveManagementSystem.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly IRepository<Guid, LeaveRequest> _leaveRequestRepository;
        private readonly ILeaveRequestCommentRepository _leaveRequestCommentRepository;
        private readonly ILeaveBalanceService _leaveBalanceService;
        private readonly IMapper _mapper;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;
        private readonly ILeaveTypeService _leaveTypeService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IServiceScopeFactory _scopeFactory;

        public LeaveRequestService(
            IRepository<Guid, LeaveRequest> leaveRequestRepository,
            ILeaveBalanceService leaveBalanceService,
            IMapper mapper,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            IUserService userService,
            ILeaveTypeService leaveTypeService,
            INotificationService notificationService,
            IHubContext<NotificationHub> hubContext,
            IServiceScopeFactory scopeFactory,
            ILeaveRequestCommentRepository leaveRequestCommentRepository
            )
        {
            _leaveRequestRepository = leaveRequestRepository;
            _leaveBalanceService = leaveBalanceService;
            _mapper = mapper;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _userService = userService;
            _leaveTypeService = leaveTypeService;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _scopeFactory = scopeFactory;
             _leaveRequestCommentRepository = leaveRequestCommentRepository;
        }

       
        public async Task<(IEnumerable<LeaveRequestResponseDto> Requests, int TotalCount)> GetAllAsync(
            int pageNumber, int pageSize, string searchTerm, string status, string sortBy, string sortOrder)
        {
            try
            {
                var requests = await _leaveRequestRepository.GetAll();
                var currentUserId = _currentUserService.GetUserId();
                var currentUserRole = _currentUserService.GetRole();

                if (currentUserRole != "HR" && currentUserRole != "Admin")
                {
                    requests = requests.Where(r => r.UserId == currentUserId);
                }

                // Search
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    requests = requests.Where(r =>
                        (!string.IsNullOrEmpty(r.Reason) && r.Reason.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                // Filter by status
                if (!string.IsNullOrWhiteSpace(status))
                {
                    requests = requests.Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
                }

                // Total count before pagination
                int totalCount = requests.Count();

                // Sorting
                requests = sortBy?.ToLower() switch
                {
                    "startdate" => (sortOrder == "desc") ? requests.OrderByDescending(r => r.StartDate) : requests.OrderBy(r => r.StartDate),
                    "enddate" => (sortOrder == "desc") ? requests.OrderByDescending(r => r.EndDate) : requests.OrderBy(r => r.EndDate),
                    "status" => (sortOrder == "desc") ? requests.OrderByDescending(r => r.Status) : requests.OrderBy(r => r.Status),
                    _ => (sortOrder == "desc") ? requests.OrderByDescending(r => r.CreatedAt) : requests.OrderBy(r => r.CreatedAt),
                };

                // Pagination
                requests = requests
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize);

                return (_mapper.Map<IEnumerable<LeaveRequestResponseDto>>(requests), totalCount);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get all leave requests", ex);
            }
        }

        public async Task<LeaveRequestResponseDto> GetByIdAsync(Guid id)
        {
            try
            {
                var request = await _leaveRequestRepository.Get(id);
                if (request == null)
                    throw new KeyNotFoundException($"Leave request with ID {id} not found");

                return _mapper.Map<LeaveRequestResponseDto>(request);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to get leave request by ID", ex);
            }
        }


        //---------------------------CREATE LEAVE REQUEST with VALIDATIONS--------------------------------

        public async Task<LeaveRequestResponseDto> CreateAsync(LeaveRequestDto dto)
        {
            try
            {
                var currentUserId = _currentUserService.GetUserId(); 

                await ValidateGenderBasedLeaveAsync(currentUserId, dto.LeaveTypeId);
                ValidateStartDate(dto.StartDate);

                var leaveType = await GetAndValidateLeaveType(dto.LeaveTypeId);

                int requestedDays = 0;
                if (!IsLeaveTypeExemptedFromConsecutiveDaysLimit(leaveType.Name)){
                    requestedDays = GetAndValidateRequestedDays(dto.StartDate, dto.EndDate);
                }
                

                if (!IsLeaveTypeExemptedFromBalanceCheck(leaveType.Name))
                {
                    await ValidateLeaveBalanceAsync(currentUserId, dto.LeaveTypeId, requestedDays, leaveType);
                    await ValidateMonthlyLimitAsync(currentUserId, dto.LeaveTypeId, dto.StartDate, requestedDays, leaveType);
                }

                await ValidateOverlappingLeaveAsync(currentUserId, dto.LeaveTypeId, dto.StartDate, dto.EndDate);

                var entity = await CreateLeaveRequestEntityAsync(dto, currentUserId);

                entity = await _leaveRequestRepository.Get(entity.Id);

                await _auditLogService.LogAsync(
                    currentUserId,
                    "INSERTED",
                    "LEAVEREQUEST",
                    entity.Id
                );
                await NotifyAllHRsOnLeaveCreationAsync(currentUserId, entity);
                return _mapper.Map<LeaveRequestResponseDto>(entity);
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }

        private void ValidateStartDate(DateTime startDate)
        {
            if (startDate.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Leave start date cannot be in the past.");
        }

        private bool IsLeaveTypeExemptedFromConsecutiveDaysLimit(string leaveTypeName)
        {
            return leaveTypeName.Contains("Maternity", StringComparison.OrdinalIgnoreCase) ||
                leaveTypeName.Contains("Paternity", StringComparison.OrdinalIgnoreCase) ||
                leaveTypeName.Contains("Marriage", StringComparison.OrdinalIgnoreCase);
        }


        private int GetAndValidateRequestedDays(DateTime startDate, DateTime endDate)
        {
            int requestedDays = GetWorkingDays(startDate, endDate);
            if (requestedDays > 15)
                throw new InvalidOperationException("You cannot apply for more than 15 consecutive leave days.");
            return requestedDays;
        }

        private async Task<LeaveType> GetAndValidateLeaveType(Guid leaveTypeId)
        {
            var leaveType = await _leaveTypeService.GetByIdAsync(leaveTypeId);
            if (leaveType == null)
                throw new InvalidOperationException("Leave type not found.");
            return leaveType;
        }

        private bool IsLeaveTypeExemptedFromBalanceCheck(string leaveTypeName)
        {
            return leaveTypeName.Contains("Sick", StringComparison.OrdinalIgnoreCase) ||
                leaveTypeName.Contains("Loss Of Pay", StringComparison.OrdinalIgnoreCase);
        }

        private async Task ValidateOverlappingLeaveAsync(Guid userId, Guid leaveTypeId, DateTime startDate, DateTime endDate)
        {
            var existingLeaves = await _leaveRequestRepository.GetAll();
            bool hasOverlap = existingLeaves.Any(l =>
                l.UserId == userId &&
                (l.Status != "Rejected" && l.Status != "Cancelled" && l.Status != "Auto-Rejected") &&
                l.StartDate <= endDate &&
                l.EndDate >= startDate);

            if (hasOverlap)
                throw new InvalidOperationException("You have already applied for leave in this date range.");
        }

        private async Task ValidateLeaveBalanceAsync(Guid userId, Guid leaveTypeId, int requestedDays, LeaveType leaveType)
        {
            var leaveBalanceResponse = await _leaveBalanceService.GetLeaveBalanceForTypeAsync(userId, leaveTypeId);
            if (leaveBalanceResponse == null || leaveBalanceResponse.LeaveBalance == null)
                throw new InvalidOperationException("Leave balance not found for this leave type.");

            int remainingDays = leaveBalanceResponse.LeaveBalance.RemainingLeaves;
            if (requestedDays > remainingDays)
                throw new InvalidOperationException($"Insufficient leave balance. Available days: {remainingDays}");
        }

        private async Task ValidateMonthlyLimitAsync(Guid userId, Guid leaveTypeId, DateTime startDate, int requestedDays, LeaveType leaveType)
        {
            int monthlyLimit = leaveType.StandardLeaveCount / 12;

            var allRequests = await _leaveRequestRepository.GetAll();
            var usedThisMonth = allRequests
                .Where(lr => lr.UserId == userId &&
                            lr.LeaveTypeId == leaveTypeId &&
                            lr.StartDate.Month == startDate.Month &&
                            lr.StartDate.Year == startDate.Year &&
                            (lr.Status == "Approved" || lr.Status == "Pending"))
                .Sum(lr => GetWorkingDays(lr.StartDate, lr.EndDate));

            if ((usedThisMonth + requestedDays) > monthlyLimit)
                throw new InvalidOperationException(
                    $"Monthly leave limit exceeded. Allowed: {monthlyLimit} days, Used: {usedThisMonth} days, Requested: {requestedDays} days.");
        }

        private async Task<LeaveRequest> CreateLeaveRequestEntityAsync(LeaveRequestDto dto, Guid userId)
        {
            var entity = _mapper.Map<LeaveRequest>(dto);
            entity.StartDate = DateTime.SpecifyKind(dto.StartDate, DateTimeKind.Utc);
            entity.EndDate = DateTime.SpecifyKind(dto.EndDate, DateTimeKind.Utc);
            entity.UserId = userId; // ✅ Correct UserId from token

            entity = await _leaveRequestRepository.Add(entity);
            return entity;
        }

        private async Task<List<Guid>> GetHRUserIdsAsync()
        {
            var result = await _userService.GetAllUsers(
                1, int.MaxValue, null, "HR", "CreatedAt", "asc"
            );

            return result.Users.Select(u => u.Id).ToList();
        }


        private async Task<string> GetCreatorNameAsync(Guid userId)
        {
            var creator = await _userService.GetUserById(userId);
            return creator.Username;
        }

        private async Task NotifyAllHRsOnLeaveCreationAsync(Guid createdById, LeaveRequest leave)
        {
            using var scope = _scopeFactory.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var result = await userService.GetAllUsers(
                1, int.MaxValue, null, "HR", "CreatedAt", "asc"
            );

            var hrIds = result.Users
                .Where(u => u.Id != createdById) 
                .Select(u => u.Id)
                .ToList();

            var creator = await userService.GetUserById(createdById);

            var message = $"📝 New leave request submitted by {creator.Username} ({creator.Email}) for " +
                        $"{leave.LeaveType?.Name ?? "Leave"} from {leave.StartDate:MMM dd} to {leave.EndDate:MMM dd}.";

            await notificationService.CreateForMultipleAsync(hrIds, message);
        }


        //---------------------------------------------------------------------------------------

        

        public async Task<bool> UpdateStatusAsync(Guid id, string status, Guid approverId)
        {
            try
            {
                var request = await _leaveRequestRepository.Get(id);
                if (request == null)
                    throw new KeyNotFoundException($"Leave request with ID {id} not found");

                if (request.Status == "Approved" || request.Status == "Rejected")
                    throw new InvalidOperationException("Cannot change status of a finalized request.");
                
                if (request.UserId == approverId)
                    throw new InvalidOperationException("You cannot approve or reject your own leave request.");

                request.Status = status;
                request.ReviewedById = approverId;
                request.UpdatedAt = DateTime.UtcNow;

                if (status == "Approved")
                {
                    int requestedDays = GetWorkingDays(request.StartDate, request.EndDate);

                    await _leaveBalanceService.DeductLeaveBalanceAsync(
                        request.UserId,
                        request.LeaveTypeId,
                        requestedDays
                    );
                }

                await _leaveRequestRepository.Update(id, request);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "UPDATED",
                    "LEAVEREQUEST",
                    request.Id
                );

                await NotifyRequesterAsync(request, status, approverId);
                await NotifyHRsAsync(request, status);
                
                return true;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message}", ex);
            }
        }

        private async Task NotifyRequesterAsync(LeaveRequest request, string status, Guid approverId)
        {
            using var scope = _scopeFactory.CreateScope();
            
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();

            var requesterId = request.UserId;
            var approver = await userService.GetUserById(approverId);
            var leaveType = request.LeaveType?.Name ?? "Leave";

            if (string.IsNullOrEmpty(leaveType))
            {
                var leaveTypeService = scope.ServiceProvider.GetRequiredService<ILeaveTypeService>();
                var leaveTypeEntity = await leaveTypeService.GetByIdAsync(request.LeaveTypeId);
                leaveType = leaveTypeEntity?.Name ?? "Leave";
            }

            var message = $"📢 Your {leaveType} leave request from {request.StartDate:MMM dd} to {request.EndDate:MMM dd} " +
                        $"has been {status.ToLower()} by {approver.Username}.";

            await notificationService.CreateAsync(requesterId, message, approverId);
        }

        private async Task<List<Guid>> GetAllHRUserIdsAsync()
        {
            var result = await _userService.GetAllUsers(
                1, int.MaxValue, null, "HR", "CreatedAt", "asc"
            );

            return result.Users.Select(u => u.Id).ToList();
        }

        private async Task<string> GetCreatorMessageAsync(Guid userId, string status)
        {
            var creator = await _userService.GetUserById(userId);
            return $"📋 {creator.Username}'s leave request was {status.ToLower()}.";
        }

        private async Task NotifyHRsAsync(LeaveRequest request, string status)
        {
            using var scope = _scopeFactory.CreateScope();

            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
            var notificationService = scope.ServiceProvider.GetRequiredService<INotificationService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<NotificationHub>>(); // Replace with your hub

            var result = await userService.GetAllUsers(1, int.MaxValue, null, "HR", "CreatedAt", "asc");
            var hrIds = result.Users.Select(u => u.Id).ToList();

            var creator = await userService.GetUserById(request.UserId);
            var approver = await userService.GetUserById(request.ReviewedById ?? Guid.Empty); // Add null check if needed

            var message = $"📋 Leave request from {creator.Username} ({creator.Email}) for {request.LeaveType?.Name ?? "N/A"} " +
                        $"from {request.StartDate:MMM dd} to {request.EndDate:MMM dd} has been {status.ToLower()} " +
                        $"by {approver?.Username ?? "an approver"}.";

            await notificationService.CreateForMultipleAsync(hrIds, message);

            foreach (var hrId in hrIds)
            {
                await hubContext.Clients.Group($"USER_{hrId}")
                    .SendAsync("ReceiveNotification", message);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var deletedItem = await _leaveRequestRepository.Delete(id);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "DELETED",
                    "LEAVEREQUEST",
                    deletedItem.Id
                );

                return true;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete leave request", ex);
            }
        }

        private async Task ValidateGenderBasedLeaveAsync(Guid userId, Guid leaveTypeId)
        {
            var user = await _userService.GetUserById(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found.");

            var leaveType = await _leaveTypeService.GetByIdAsync(leaveTypeId);
            if (leaveType == null)
                throw new KeyNotFoundException($"Leave Type with ID {leaveTypeId} not found.");

            var gender = user.Gender?.ToLower();
            var leaveTypeName = leaveType.Name?.ToLower();

            if (leaveTypeName.Contains("maternity") && gender != "female")
                throw new InvalidOperationException("Only female employees can apply for Maternity Leave.");

            if (leaveTypeName.Contains("paternity") && gender != "male")
                throw new InvalidOperationException("Only male employees can apply for Paternity Leave.");
        }

        private int GetWorkingDays(DateTime start, DateTime end)
        {
            int totalDays = 0;

            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    totalDays++;
                }
            }

            return totalDays;
        }


        public async Task<bool> CancelLeaveRequestAsync(Guid id)
        {
            var currentUserId = _currentUserService.GetUserId();

            // Step 1: Get the leave request
            var request = await _leaveRequestRepository.Get(id);
            if (request == null)
                throw new KeyNotFoundException($"Leave request with ID {id} not found.");

            if (request.UserId != currentUserId)
                throw new UnauthorizedAccessException("You can only cancel your own leave requests.");

            if (request.Status == "Approved")
                throw new InvalidOperationException("Approved leave requests cannot be cancelled.");

            if (request.Status == "Rejected")
                throw new InvalidOperationException("Rejected leave requests do not need cancellation.");

            // Step 2: Update request
            request.Status = "Cancelled";
            request.UpdatedAt = DateTime.UtcNow;
            await _leaveRequestRepository.Update(id, request);

            // Step 4: Notify user
            var userMessage = $"❌ You cancelled your leave request from {request.StartDate:MMM dd} to {request.EndDate:MMM dd}.";
            await _notificationService.CreateAsync(currentUserId, userMessage);

            // Step 5: Get HRs first
            // var hrUsersResult = await _userService.GetAllUsers(
            //     1, int.MaxValue, null, "HR", "CreatedAt", "asc"
            // );
            // var hrUserIds = hrUsersResult.Users
            //     .Where(hr => hr.Id != currentUserId)
            //     .Select(hr => hr.Id)
            //     .ToList();

            // // Step 6: Get current user data
            // var creator = await _userService.GetUserById(currentUserId);
            // var hrMessage = $" {creator.Username} ({creator.Email}) has cancelled their leave request from {request.StartDate:MMM dd} to {request.EndDate:MMM dd}.";

            // // Step 7: Notify HRs
            // await _notificationService.CreateForMultipleAsync(hrUserIds, hrMessage);

            return true;
        }
        public async Task<bool> AdminOverrideLeaveStatusAsync(Guid leaveRequestId, string newStatus, Guid adminId, string? comment = null)
{
    var leaveRequest = await _leaveRequestRepository.Get(leaveRequestId);
    if (leaveRequest == null)
        throw new KeyNotFoundException($"Leave request with ID {leaveRequestId} not found");

    if (leaveRequest.Status != "Approved" && leaveRequest.Status != "Rejected")
        throw new InvalidOperationException("Only approved or rejected leave requests can be overridden by admin.");

    if (leaveRequest.UserId == adminId)
        throw new InvalidOperationException("Admin cannot override their own leave request.");

    // Update status
    leaveRequest.Status = newStatus;
    leaveRequest.ReviewedById = adminId;
    leaveRequest.UpdatedAt = DateTime.UtcNow;

    if (newStatus == "Approved")
    {
        int requestedDays = GetWorkingDays(leaveRequest.StartDate, leaveRequest.EndDate);
        await _leaveBalanceService.DeductLeaveBalanceAsync(
            leaveRequest.UserId,
            leaveRequest.LeaveTypeId,
            requestedDays
        );
    }

    await _leaveRequestRepository.Update(leaveRequestId, leaveRequest);

    // Save comment if provided
    if (!string.IsNullOrWhiteSpace(comment))
    {
        var commentEntry = new LeaveRequestComment
        {
            Id = Guid.NewGuid(),
            LeaveRequestId = leaveRequestId,
            AdminId = adminId,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        await _leaveRequestCommentRepository.Add(commentEntry);
    }

    await _auditLogService.LogAsync(adminId, "ADMIN OVERRIDE", "LEAVEREQUEST", leaveRequestId);
    await NotifyRequesterAsync(leaveRequest, newStatus, adminId);

    return true;
}

    }
}
