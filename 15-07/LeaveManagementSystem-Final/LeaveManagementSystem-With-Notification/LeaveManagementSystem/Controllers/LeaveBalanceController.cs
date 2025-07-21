using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v1/leave-balance")]
    public class LeaveBalanceController : ControllerBase
    {
        private readonly ILeaveBalanceService _leaveBalanceService;
        private readonly ILogger<LeaveBalanceController> _logger;

        public LeaveBalanceController(ILeaveBalanceService leaveBalanceService, ILogger<LeaveBalanceController> logger)
        {
            _leaveBalanceService = leaveBalanceService;
            _logger = logger;
        }

        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<UserLeaveBalanceResponseDto>>> GetLeaveBalancesForUser(Guid userId)
        {
            try
            {
                _logger.LogInformation("Fetching leave balances for user ID: {UserId}", userId);
                var result = await _leaveBalanceService.GetLeaveBalancesForUserAsync(userId);
                return Ok(ApiResponse<UserLeaveBalanceResponseDto>.SuccessResponse(result, "Leave balances fetched successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Leave balances not found for user ID: {UserId}. {Message}", userId, ex.Message);
                return NotFound(ApiResponse<UserLeaveBalanceResponseDto>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave balances for user ID: {UserId}", userId);
                return StatusCode(500, ApiResponse<UserLeaveBalanceResponseDto>.FailureResponse("An error occurred while fetching leave balances."));
            }
        }

        [Authorize]
        [HttpGet("user/{userId}/leaveType/{leaveTypeId}")]
        public async Task<ActionResult<ApiResponse<UserLeaveBalanceForTypeResponseDto>>> GetLeaveBalanceForUserByType(Guid userId, Guid leaveTypeId)
        {
            try
            {
                _logger.LogInformation("Fetching leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId}", userId, leaveTypeId);
                var result = await _leaveBalanceService.GetLeaveBalanceForTypeAsync(userId, leaveTypeId);
                return Ok(ApiResponse<UserLeaveBalanceForTypeResponseDto>.SuccessResponse(result, "Leave balance fetched successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Leave balance not found for user ID: {UserId} and leave type ID: {LeaveTypeId}. {Message}", userId, leaveTypeId, ex.Message);
                return NotFound(ApiResponse<UserLeaveBalanceForTypeResponseDto>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId}", userId, leaveTypeId);
                return StatusCode(500, ApiResponse<UserLeaveBalanceForTypeResponseDto>.FailureResponse("An error occurred while fetching leave balance."));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost("initialize/user/{userId}")]
        public async Task<ActionResult<ApiResponse<string>>> InitializeLeaveBalancesForUser(Guid userId)
        {
            try
            {
                _logger.LogInformation("Initializing leave balances for user ID: {UserId}", userId);
                await _leaveBalanceService.InitializeLeaveBalancesForUserAsync(userId);
                return Ok(ApiResponse<string>.SuccessResponse("Leave balances initialized for the user."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing leave balances for user ID: {UserId}", userId);
                return StatusCode(500, ApiResponse<string>.FailureResponse("An error occurred while initializing leave balances."));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost("initialize/user/{userId}/leaveType/{leaveTypeId}")]
        public async Task<ActionResult<ApiResponse<string>>> InitializeLeaveBalanceForNewLeaveType(Guid userId, Guid leaveTypeId, [FromQuery] int standardLeaveCount)
        {
            if (standardLeaveCount < 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Standard leave count cannot be negative."));
            }

            try
            {
                _logger.LogInformation("Initializing leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId} with standard leave count: {StandardLeaveCount}", userId, leaveTypeId, standardLeaveCount);
                await _leaveBalanceService.InitializeLeaveBalanceForNewLeaveTypeAsync(userId, leaveTypeId, standardLeaveCount);
                return Ok(ApiResponse<string>.SuccessResponse("Leave balance initialized for new leave type."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId}", userId, leaveTypeId);
                return StatusCode(500, ApiResponse<string>.FailureResponse("An error occurred while initializing leave balance."));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost("deduct")]
        public async Task<ActionResult<ApiResponse<string>>> DeductLeaveBalance([FromQuery] Guid userId, [FromQuery] Guid leaveTypeId, [FromQuery] int days)
        {
            if (days <= 0)
            {
                return BadRequest(ApiResponse<string>.FailureResponse("Days to deduct must be greater than zero."));
            }

            try
            {
                _logger.LogInformation("Deducting {Days} days from leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId}", days, userId, leaveTypeId);
                await _leaveBalanceService.DeductLeaveBalanceAsync(userId, leaveTypeId, days);
                return Ok(ApiResponse<string>.SuccessResponse("Leave balance deducted successfully."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deducting leave balance for user ID: {UserId} and leave type ID: {LeaveTypeId}", userId, leaveTypeId);
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message));
            }
        }

        [Authorize(Roles = "HR")]
        [HttpPost("reset/user/{userId}")]
        public async Task<ActionResult<ApiResponse<string>>> ResetLeaveBalancesForUser(Guid userId)
        {
            try
            {
                _logger.LogInformation("Resetting leave balances for user ID: {UserId}", userId);
                await _leaveBalanceService.ResetLeaveBalancesForUserAsync(userId);
                return Ok(ApiResponse<string>.SuccessResponse("Leave balances reset for the user."));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting leave balances for user ID: {UserId}", userId);
                return StatusCode(500, ApiResponse<string>.FailureResponse("An error occurred while resetting leave balances."));
            }
        }
    }
}
