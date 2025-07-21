// using LeaveManagementSystem.Interfaces;
// using LeaveManagementSystem.Models.DTOs;
// using LeaveManagementSystem.Responses;
// using Microsoft.AspNetCore.Authorization;
// using Microsoft.AspNetCore.Mvc;
// using System.Security.Claims;

// namespace LeaveManagementSystem.Controllers
// {
//     [ApiController]
//     [Route("api/v{version:apiVersion}/admin/leave")]
//     [ApiVersion("1.0")]
//     public class AdminLeaveController : ControllerBase
//     {
//         private readonly ILeaveRequestService _leaveService;
//         private readonly ILogger<AdminLeaveController> _logger;

//         public AdminLeaveController(ILeaveRequestService leaveService, ILogger<AdminLeaveController> logger)
//         {
//             _leaveService = leaveService;
//             _logger = logger;
//         }

//         [HttpPost("{id:guid}/override")]
//         [Authorize(Roles = "Admin")]
//         public async Task<IActionResult> OverrideLeaveRequest(Guid id, [FromBody] AdminOverrideLeaveRequestDto dto)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ApiResponse<object>.FailureResponse("Invalid input", ModelStateErrors()));
//             }

//             var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//             if (!Guid.TryParse(userIdClaim, out var adminId))
//             {
//                 return Unauthorized(ApiResponse<object>.FailureResponse("Invalid admin identity"));
//             }

//             try
//             {
//                 await _leaveService.AdminOverrideLeaveStatusAsync(id, dto.NewStatus, adminId, dto.Comment);
//                 return Ok(ApiResponse<object>.SuccessResponse(null, "Leave request overridden successfully by Admin."));
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "Admin override failed for leave request {LeaveRequestId}", id);
//                 return StatusCode(500, ApiResponse<object>.FailureResponse("Override failed", new Dictionary<string, List<string>>
//                 {
//                     { "Exception", new List<string> { ex.Message } }
//                 }));
//             }
//         }

//         private Dictionary<string, List<string>> ModelStateErrors()
//         {
//             var errors = new Dictionary<string, List<string>>();
//             foreach (var key in ModelState.Keys)
//             {
//                 var errorMessages = ModelState[key].Errors;
//                 if (errorMessages.Count > 0)
//                 {
//                     errors[key] = new List<string>();
//                     foreach (var error in errorMessages)
//                     {
//                         errors[key].Add(error.ErrorMessage);
//                     }
//                 }
//             }
//             return errors;
//         }
//     }
// }
