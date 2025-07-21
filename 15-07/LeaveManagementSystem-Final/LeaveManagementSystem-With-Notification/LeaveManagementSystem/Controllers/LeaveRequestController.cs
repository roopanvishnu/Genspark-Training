using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v1/leave-requests")]
    [Authorize]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _service;
        private readonly ILogger<LeaveRequestController> _logger;

        public LeaveRequestController(ILeaveRequestService service, ILogger<LeaveRequestController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string searchTerm = null,
            [FromQuery] string status = null,
            [FromQuery] string sortBy = "CreatedAt",
            [FromQuery] string sortOrder = "asc"
        )
        {
            pageSize = pageSize > 100 ? 100 : pageSize;

            try
            {
                _logger.LogInformation("Fetching all leave requests with pagination, search, filter, sort parameters.");

                var (leaveRequests, totalCount) = await _service.GetAllAsync(
                    page, pageSize, searchTerm, status, sortBy, sortOrder
                );

                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var response = new
                {
                    data = leaveRequests,
                    pagination = new
                    {
                        totalRecords = totalCount,
                        page,
                        pageSize,
                        totalPages
                    }
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Leave requests fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching paginated leave requests.");
                return StatusCode(500, ApiResponse<object>.FailureResponse($"Error fetching leave requests: {ex.Message}"));
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching leave request with ID: {LeaveRequestId}", id);
                var request = await _service.GetByIdAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(request, "Leave request fetched successfully"));
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Leave request with ID: {LeaveRequestId} not found. {Message}", id, knf.Message);
                return NotFound(ApiResponse<object>.FailureResponse(knf.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave request with ID: {LeaveRequestId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse($"Error fetching leave request: {ex.Message}"));
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] LeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for leave request creation.");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid request", ModelStateErrors()));
            }

            try
            {
                _logger.LogInformation("Creating new leave request.");
                dto.Status = "Pending";
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<object>.SuccessResponse(created, "Leave request created successfully"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Validation failed during leave request creation.");
                return BadRequest(ApiResponse<object>.FailureResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave request.");
                return StatusCode(500, ApiResponse<object>.FailureResponse($"{ex.Message}"));
            }
        }


        [HttpPut("{id}/status")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateLeaveRequestStatusDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating leave request status.");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid request", ModelStateErrors()));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var approverId))
            {
                _logger.LogWarning("Invalid user identity while updating leave request status.");
                return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user identity"));
            }

            try
            {
                _logger.LogInformation("Updating leave request status for ID: {LeaveRequestId} by approver ID: {ApproverId}", id, approverId);
                var success = await _service.UpdateStatusAsync(id, dto.Status, approverId);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Leave request status updated successfully"));
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Leave request with ID: {LeaveRequestId} not found for status update. {Message}", id, knf.Message);
                return NotFound(ApiResponse<object>.FailureResponse(knf.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leave request status for ID: {LeaveRequestId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse($"{ex.Message}"));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting leave request with ID: {LeaveRequestId}", id);
                var success = await _service.DeleteAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Leave request deleted successfully"));
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Leave request with ID: {LeaveRequestId} not found for deletion. {Message}", id, knf.Message);
                return NotFound(ApiResponse<object>.FailureResponse(knf.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting leave request with ID: {LeaveRequestId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse($"Error deleting leave request: {ex.Message}"));
            }
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelLeaveRequest(Guid id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                _logger.LogWarning("Invalid user identity during leave request cancellation.");
                return Unauthorized(ApiResponse<object>.FailureResponse("Invalid user identity"));
            }

            try
            {
                _logger.LogInformation("User {UserId} requested cancellation for leave request ID: {LeaveRequestId}", userId, id);
                var success = await _service.CancelLeaveRequestAsync(id);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Leave request cancelled successfully"));
            }
            catch (KeyNotFoundException knf)
            {
                _logger.LogWarning("Leave request not found for cancellation: {Message}", knf.Message);
                return NotFound(ApiResponse<object>.FailureResponse(knf.Message));
            }
            catch (InvalidOperationException ioe)
            {
                _logger.LogWarning("Leave cancellation invalid operation: {Message}", ioe.Message);
                return BadRequest(ApiResponse<object>.FailureResponse(ioe.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling leave request with ID: {LeaveRequestId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse($"Error cancelling leave request: {ex.Message}"));
            }
        }
        [HttpPut("{id}/admin-override")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminOverrideLeaveStatus(Guid id, [FromBody] AdminOverrideLeaveRequestDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model for admin override.");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid request", ModelStateErrors()));
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out var adminId))
            {
                _logger.LogWarning("Invalid user identity for admin override.");
                return Unauthorized(ApiResponse<object>.FailureResponse("Invalid admin identity"));
            }

            try
            {
                var success = await _service.AdminOverrideLeaveStatusAsync(id, dto.NewStatus, adminId, dto.Comment);
                return Ok(ApiResponse<object>.SuccessResponse(null, "Leave request overridden by admin"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin override failed for leave request ID: {LeaveRequestId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse(ex.Message));
            }
        }


        private Dictionary<string, List<string>> ModelStateErrors()
        {
            var errors = new Dictionary<string, List<string>>();
            foreach (var key in ModelState.Keys)
            {
                var errorMessages = ModelState[key].Errors;
                if (errorMessages.Count > 0)
                {
                    errors[key] = new List<string>();
                    foreach (var error in errorMessages)
                    {
                        errors[key].Add(error.ErrorMessage);
                    }
                }
            }
            return errors;
        }
    }
}
