using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging; // For logging
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v1/leavetypes")]
    public class LeaveTypeController : ControllerBase
    {
        private readonly ILeaveTypeService _service;
        private readonly ILogger<LeaveTypeController> _logger; 

        public LeaveTypeController(ILeaveTypeService service, ILogger<LeaveTypeController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000,
                                                [FromQuery] string searchTerm = null, [FromQuery] string sortBy = null,
                                                [FromQuery] string sortOrder = "asc")
        {
            try
            {
                _logger.LogInformation("Fetching all leave types with pagination.");
                var result = await _service.GetAllAsync(pageNumber, pageSize, searchTerm, sortBy, sortOrder);
                return Ok(ApiResponse<IEnumerable<LeaveTypeResponseDto>>.SuccessResponse(result));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave types.");
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while fetching leave types."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching leave type with ID: {LeaveTypeId}", id);
                var result = await _service.GetByIdAsync(id);
                return Ok(ApiResponse<LeaveType>.SuccessResponse(result, "Leave type fetched successfully"));
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Leave type with ID: {LeaveTypeId} not found.", id);
                return NotFound(ApiResponse<LeaveTypeDto>.FailureResponse("Leave type not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching leave type with ID: {LeaveTypeId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while fetching the leave type."));
            }
        }

        [HttpPost]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Create([FromBody] LeaveTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid leave type creation request.");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid data", ModelStateErrors()));
            }

            try
            {
                _logger.LogInformation("Creating new leave type.");
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = created.Id },
                    ApiResponse<LeaveType>.SuccessResponse(created, "Leave type created successfully")
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating leave type.");
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while creating the leave type."));
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Update(Guid id, [FromBody] LeaveTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid leave type update request.");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid data", ModelStateErrors()));
            }

            try
            {
                _logger.LogInformation("Updating leave type with ID: {LeaveTypeId}", id);
                var updated = await _service.UpdateAsync(id, dto);
                return Ok(ApiResponse<LeaveType>.SuccessResponse(updated, "Leave type updated successfully"));
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Leave type with ID: {LeaveTypeId} not found for update.", id);
                return NotFound(ApiResponse<LeaveType>.FailureResponse("Leave type not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating leave type with ID: {LeaveTypeId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while updating the leave type."));
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "HR")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting leave type with ID: {LeaveTypeId}", id);
                var success = await _service.DeleteAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Leave type with ID: {LeaveTypeId} not found for deletion.", id);
                    return NotFound(ApiResponse<object>.FailureResponse("Leave type not found"));
                }

                return Ok(ApiResponse<LeaveType>.SuccessResponse(null, "Leave type deleted successfully"));
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Leave type with ID: {LeaveTypeId} not found for deletion.", id);
                return NotFound(ApiResponse<object>.FailureResponse("Leave type not found"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting leave type with ID: {LeaveTypeId}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while deleting the leave type."));
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
