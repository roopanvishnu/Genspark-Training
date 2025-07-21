using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Controllers
{
    [ApiController]
    [Route("api/v1/leave-attachments")]
    public class LeaveAttachmentController : ControllerBase
    {
        private readonly ILeaveAttachmentService _service;
        private readonly ILogger<LeaveAttachmentController> _logger; 

        public LeaveAttachmentController(ILeaveAttachmentService service, ILogger<LeaveAttachmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet("by-leave-request/{leaveRequestId}")]
        public async Task<IActionResult> GetByLeaveRequestId(Guid leaveRequestId)
        {
            try
            {
                _logger.LogInformation("Fetching attachments for LeaveRequestId: {LeaveRequestId}", leaveRequestId);
                var attachments = await _service.GetByLeaveRequestIdAsync(leaveRequestId);

                if (attachments == null || !attachments.Any())
                {
                    _logger.LogWarning("No attachments found for LeaveRequestId: {LeaveRequestId}", leaveRequestId);
                    return NotFound(ApiResponse<object>.FailureResponse("No attachments found"));
                }

                var response = attachments.Select(a => new LeaveAttachmentResponse
                {
                    Id = a.Id,
                    LeaveRequestId = a.LeaveRequestId,
                    FileName = a.FileName,
                    UploadedAt = a.UploadedAt,
                    DownloadUrl = $"{Request.Scheme}://{Request.Host}/api/v1/leave-attachments/{a.Id}"
                });

                return Ok(ApiResponse<IEnumerable<LeaveAttachmentResponse>>.SuccessResponse(response, "Leave attachments fetched successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attachments for LeaveRequestId: {LeaveRequestId}", leaveRequestId);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while fetching attachments."));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                _logger.LogInformation("Fetching attachment with ID: {Id}", id);
                var attachment = await _service.GetByIdAsync(id);

                if (attachment == null)
                {
                    _logger.LogWarning("Attachment not found with ID: {Id}", id);
                    return NotFound(ApiResponse<object>.FailureResponse("Leave attachment not found"));
                }

                return File(attachment.FileContent, GetContentType(attachment.FileName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching attachment with ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while fetching the attachment."));
            }
        }

        private string GetContentType(string fileName)
        {
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(fileName, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] LeaveAttachmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid LeaveAttachmentDto input");
                return BadRequest(ApiResponse<object>.FailureResponse("Invalid input"));
            }

            try
            {
                _logger.LogInformation("Creating new leave attachment");
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id },
                    ApiResponse<object>.SuccessResponse(created, "Leave attachment created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating new leave attachment");
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while creating the leave attachment."));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                _logger.LogInformation("Deleting leave attachment with ID: {Id}", id);
                var success = await _service.DeleteAsync(id);

                if (!success)
                {
                    _logger.LogWarning("Leave attachment not found with ID: {Id}", id);
                    return NotFound(ApiResponse<object>.FailureResponse("Leave attachment not found"));
                }

                return Ok(ApiResponse<object>.SuccessResponse(null, "Leave attachment deleted successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting leave attachment with ID: {Id}", id);
                return StatusCode(500, ApiResponse<object>.FailureResponse("An error occurred while deleting the leave attachment."));
            }
        }
    }
}
