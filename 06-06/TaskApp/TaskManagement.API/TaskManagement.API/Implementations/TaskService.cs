

using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Context;
using TaskManagement.API.DTOs.TaskDtos;
using TaskManagement.API.Hubs;
using TaskManagement.API.Models;
using TaskManagement.API.Models;
using TaskManagement.API.Interfaces;

namespace TaskManagement.API.Services.Implementations
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;
        private readonly IHubContext<TaskHub> _hubContext;

        private readonly string _basePath = "/Users/roopanvishnu/Downloads/taskappfiles";
        private object _logger;

        public TaskService(AppDbContext context, IMapper mapper, IWebHostEnvironment env, IHubContext<TaskHub> hubContext)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
            _hubContext = hubContext;
        }

        public async Task<TaskDto?> CreateTaskAsync(CreateTaskDto dto, string createdBy)
        {
            if (dto.AssigneeId.HasValue && !await _context.Users.AnyAsync(u => u.Id == dto.AssigneeId.Value && !u.IsDeleted))
                return null;

            var task = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                AssigneeId = dto.AssigneeId,
                DueDate = dto.DueDate?.ToUniversalTime()
            };

            if (dto.Attachment != null && dto.Attachment.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{dto.Attachment.FileName}";
                var filePath = Path.Combine(_basePath, fileName);

                Directory.CreateDirectory(_basePath);

                using var stream = new FileStream(filePath, FileMode.Create);
                await dto.Attachment.CopyToAsync(stream);

                task.Attachments = new List<TaskAttachment>
                {
                    new TaskAttachment
                    {
                        Id = Guid.NewGuid(),
                        FileName = dto.Attachment.FileName,
                        FilePath = filePath,
                        UploadedAt = DateTime.UtcNow
                    }
                };
            }

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // SignalR broadcast for task creation
            await _hubContext.Clients.All.SendAsync("taskCreated", new {
                taskId = task.Id,
                title = task.Title
            });

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<FileStreamResult?> DownloadAttachmentAsync(Guid taskId, string currentUserId, string role)
        {
            var task = await _context.Tasks
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            if (task == null)
                return null;

            if (role != "Manager" && task.AssigneeId?.ToString() != currentUserId)
                return null;

            var attachment = task.Attachments.FirstOrDefault();
            if (attachment == null || !System.IO.File.Exists(attachment.FilePath))
                return null;

            var stream = new FileStream(attachment.FilePath, FileMode.Open, FileAccess.Read);
            var fileExt = Path.GetExtension(attachment.FilePath).ToLower();

            var mime = fileExt switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };

            return new FileStreamResult(stream, mime)
            {
                FileDownloadName = attachment.FileName
            };
        }

        public async Task<bool> UpdateTaskAsync(Guid id, UpdateTaskDto dto, string updatedBy)
{
    var task = await _context.Tasks
        .Include(t => t.Attachments)
        .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

    if (task == null)
        return false;

    // Update basic fields
    if (!string.IsNullOrWhiteSpace(dto.Title))
        task.Title = dto.Title;

    if (!string.IsNullOrWhiteSpace(dto.Description))
        task.Description = dto.Description;

    if (!string.IsNullOrWhiteSpace(dto.Status))
        task.Status = dto.Status;

    // Safely convert due date to UTC
    if (dto.DueDate.HasValue)
    {
        task.DueDate = dto.DueDate.Value.Kind switch
        {
            DateTimeKind.Unspecified => DateTime.SpecifyKind(dto.DueDate.Value, DateTimeKind.Local).ToUniversalTime(),
            DateTimeKind.Local => dto.DueDate.Value.ToUniversalTime(),
            DateTimeKind.Utc => dto.DueDate.Value,
            _ => dto.DueDate.Value.ToUniversalTime()
        };
    }

    // Handle optional new attachment
    if (dto.Attachment != null && dto.Attachment.Length > 0)
    {
        // Delete old attachment (if any)
        var oldAttachment = task.Attachments.FirstOrDefault();
        if (oldAttachment != null)
        {
            if (File.Exists(oldAttachment.FilePath))
                File.Delete(oldAttachment.FilePath);

            _context.Attach(oldAttachment); // Ensure it's tracked before removal
            _context.TaskAttachments.Remove(oldAttachment);
        }

        // Save new file
        var fileName = $"{Guid.NewGuid()}_{dto.Attachment.FileName}";
        var filePath = Path.Combine(_basePath, fileName);
        Directory.CreateDirectory(_basePath);

        using var stream = new FileStream(filePath, FileMode.Create);
        await dto.Attachment.CopyToAsync(stream);

        // Add new attachment to the same collection (don't reassign list)
        task.Attachments.Clear();
        task.Attachments.Add(new TaskAttachment
        {
            Id = Guid.NewGuid(),
            FileName = dto.Attachment.FileName,
            FilePath = filePath,
            UploadedAt = DateTime.UtcNow
        });
    }

    // Audit fields
    task.UpdatedAt = DateTime.UtcNow;
    task.UpdatedBy = updatedBy;

    // Save
    await _context.SaveChangesAsync();

    // Notify via SignalR
    await _hubContext.Clients.All.SendAsync("taskUpdated", new
    {
        taskId = task.Id,
        status = task.Status
    });

    return true;
}
        public async Task<bool> DeleteTaskAsync(Guid id, string deletedBy)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null)
                return false;

            task.IsDeleted = true;
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = deletedBy;

            await _context.SaveChangesAsync();

            // Optional: SignalR broadcast for task deletion
            await _hubContext.Clients.All.SendAsync("taskDeleted", new
            {
                taskId = task.Id
            });

            return true;
        }



        public async Task<TaskDto?> GetTaskByIdAsync(Guid id, string userId, string role)
        {
            var task = await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);

            if (task == null) return null;

            if (role != "Manager" && task.AssigneeId?.ToString() != userId)
                return null;

            return _mapper.Map<TaskDto>(task);
        }

        public async Task<(List<TaskDto> tasks, int total)> GetTasksAsync(string? status, Guid? assigneeId, int page, int pageSize)
        {
            var query = _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Attachments)
                .Where(t => !t.IsDeleted);

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(t => t.Status.ToLower() == status.ToLower());

            if (assigneeId.HasValue)
                query = query.Where(t => t.AssigneeId == assigneeId);

            var total = await query.CountAsync();
            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (_mapper.Map<List<TaskDto>>(tasks), total);
        }
        public async Task<List<TaskDto>> GetDeletedTasksAsync()
        {
            var tasks = await _context.Tasks
                .IgnoreQueryFilters() // ✅ This bypasses global "IsDeleted = false"
                .Include(t => t.Assignee)
                .Include(t => t.Attachments)
                .Where(t => t.IsDeleted) // ✅ Now this will actually work
                .ToListAsync();

            return _mapper.Map<List<TaskDto>>(tasks);
        }



        public async Task<List<TaskDto>> GetAssignedTasksAsync(string currentUserId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Attachments)
                .Where(t => 
                    (!t.IsDeleted) &&
                    (t.AssigneeId.ToString() == currentUserId || t.IsBroadcasted))
                .ToListAsync();

            return _mapper.Map<List<TaskDto>>(tasks);
        }


        public async Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId, string assignerId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == "TeamMember" && !u.IsDeleted);

            if (task == null || user == null)
                return false;

            task.AssigneeId = userId;
            task.UpdatedBy = assignerId;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // SignalR notify
            await _hubContext.Clients.All.SendAsync("taskAssigned", new
            {
                taskId = task.Id,
                assignee = user.FullName
            });

            return true;
        }


        public async Task<List<string>?> GetAttachmentsAsync(Guid taskId, string userId, string role)
        {
            var task = await _context.Tasks
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            if (task == null || (role != "Manager" && task.AssigneeId?.ToString() != userId))
                return null;

            return task.Attachments.Select(a => a.FileName).ToList();
        }
        
        public async Task<bool> UploadAttachmentAsync(Guid taskId, IFormFile file, string userId)
        {
            try
            {
                var task = await _context.Tasks.Include(t => t.Attachments).FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
                if (task == null) 
                {
                    Console.WriteLine($"Task not found: {taskId}");
                    return false;
                }

                // Create uploads directory if it doesn't exist
                var uploadsPath = Path.Combine(_env.WebRootPath ?? _env.ContentRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                    Console.WriteLine($"Created uploads directory: {uploadsPath}");
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                var filePath = Path.Combine(uploadsPath, fileName);

                Console.WriteLine($"Saving file to: {filePath}");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Initialize Attachments collection if null
                if (task.Attachments == null)
                {
                    task.Attachments = new List<TaskAttachment>();
                }

                task.Attachments.Add(new TaskAttachment
                {
                    Id = Guid.NewGuid(),
                    FileName = file.FileName,
                    FilePath = filePath,
                    UploadedAt = DateTime.UtcNow
                });

                var saveResult = await _context.SaveChangesAsync();
                Console.WriteLine($"Save result: {saveResult} changes saved");
        
                return saveResult > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Upload error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return false;
            }
        }


        public async Task<FileStreamResult?> DownloadFileByNameAsync(string filename)
        {
            var filePath = Path.Combine(_basePath, filename);
            if (!File.Exists(filePath)) return null;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var mime = Path.GetExtension(filename).ToLower() switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };

            return new FileStreamResult(stream, mime)
            {
                FileDownloadName = filename
            };
        }
        
        public async Task<bool> UpdateStatusByTeamMemberAsync(Guid taskId, string teamMemberId, string status, string? comment)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
                return false;

            var isAssigned = task.AssigneeId != null && task.AssigneeId == Guid.Parse(teamMemberId);
            var isBroadcasted = task.IsBroadcasted;

            if (!isAssigned && !isBroadcasted)
                return false;

            task.Status = status;

            if (!string.IsNullOrWhiteSpace(comment))
                task.TeamComment = comment;

            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All.SendAsync("taskUpdated", new
            {
                taskId = task.Id,
                status = task.Status
            });

            return true;
        }



        public async Task<int> BroadcastTaskToAllAsync(Guid taskId, string assignerId)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            if (task == null) return 0;

            task.IsBroadcasted = true;
            task.AssigneeId = null; // Optional: remove individual assignment
            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = assignerId;

            await _context.SaveChangesAsync();

            var teamCount = await _context.Users.CountAsync(u => u.Role == "TeamMember" && !u.IsDeleted);

            // SignalR
            await _hubContext.Clients.All.SendAsync("tasksBroadcast", new
            {
                taskId = task.Id,
                title = task.Title,
                assignedCount = teamCount
            });

            return teamCount;
        }


    }
}

