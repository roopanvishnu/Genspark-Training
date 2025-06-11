using AutoMapper;
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
                AssigneeId = dto.AssigneeId
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

            if (!string.IsNullOrWhiteSpace(dto.Title))
                task.Title = dto.Title;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                task.Description = dto.Description;

            if (!string.IsNullOrWhiteSpace(dto.Status))
                task.Status = dto.Status;

            if (dto.DueDate.HasValue)
                task.DueDate = dto.DueDate;

            // Handle optional attachment
            if (dto.Attachment != null && dto.Attachment.Length > 0)
            {
                // Delete existing file from disk
                var oldAttachment = task.Attachments.FirstOrDefault();
                if (oldAttachment != null && File.Exists(oldAttachment.FilePath))
                {
                    File.Delete(oldAttachment.FilePath);
                    _context.Remove(oldAttachment);
                }

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

            task.UpdatedAt = DateTime.UtcNow;
            task.UpdatedBy = updatedBy;

            await _context.SaveChangesAsync();

            // SignalR broadcast for task update
            await _hubContext.Clients.All.SendAsync("taskUpdated", new {
                taskId = task.Id,
                status = task.Status
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

        public async Task<List<TaskDto>> GetAssignedTasksAsync(string currentUserId)
        {
            var tasks = await _context.Tasks
                .Include(t => t.Assignee)
                .Include(t => t.Attachments)
                .Where(t => t.AssigneeId.ToString() == currentUserId && !t.IsDeleted)
                .ToListAsync();

            return _mapper.Map<List<TaskDto>>(tasks);
        }

        public async Task<bool> AssignTaskToUserAsync(Guid taskId, Guid userId, string assignerId)
        {
            var task = await _context.Tasks
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.Role == "TeamMember" && !u.IsDeleted);

            if (task == null || user == null)
                return false;

            // Clone task for that user
            var newTask = new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = task.Title,
                Description = task.Description,
                Status = "Open",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = assignerId,
                AssigneeId = userId
            };

            if (task.Attachments?.Any() == true)
            {
                var orig = task.Attachments.First();
                var newFileName = $"{Guid.NewGuid()}_{orig.FileName}";
                var newPath = Path.Combine(_basePath, newFileName);

                File.Copy(orig.FilePath, newPath);

                newTask.Attachments = new List<TaskAttachment>
                {
                    new TaskAttachment
                    {
                        Id = Guid.NewGuid(),
                        FileName = orig.FileName,
                        FilePath = newPath,
                        UploadedAt = DateTime.UtcNow
                    }
                };
            }

            _context.Tasks.Add(newTask);
            await _context.SaveChangesAsync();

            // SignalR broadcast for task assignment
            await _hubContext.Clients.All.SendAsync("taskAssigned", new {
                taskId = newTask.Id,
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
            var task = await _context.Tasks.Include(t => t.Attachments).FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);
            if (task == null || task.CreatedBy != userId) return false;

            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_basePath, fileName);

            Directory.CreateDirectory(_basePath);
            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            task.Attachments.Add(new TaskAttachment
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                FilePath = filePath,
                UploadedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
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

        public async Task<int> BroadcastTaskToAllAsync(Guid taskId, string assignerId)
        {
            var task = await _context.Tasks
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == taskId && !t.IsDeleted);

            if (task == null) return 0;

            var teamMembers = await _context.Users
                .Where(u => u.Role == "TeamMember" && !u.IsDeleted)
                .ToListAsync();

            int count = 0;

            foreach (var member in teamMembers)
            {
                var newTask = new TaskItem
                {
                    Id = Guid.NewGuid(),
                    Title = task.Title,
                    Description = task.Description,
                    Status = "Open",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = assignerId,
                    AssigneeId = member.Id
                };

                if (task.Attachments?.Any() == true)
                {
                    var orig = task.Attachments.First();
                    var newFileName = $"{Guid.NewGuid()}_{orig.FileName}";
                    var newPath = Path.Combine(_basePath, newFileName);

                    File.Copy(orig.FilePath, newPath);

                    newTask.Attachments = new List<TaskAttachment>
                    {
                        new TaskAttachment
                        {
                            Id = Guid.NewGuid(),
                            FileName = orig.FileName,
                            FilePath = newPath,
                            UploadedAt = DateTime.UtcNow
                        }
                    };
                }

                _context.Tasks.Add(newTask);
                count++;
            }

            await _context.SaveChangesAsync();

            // SignalR broadcast for bulk task assignment
            await _hubContext.Clients.All.SendAsync("tasksBroadcast", new {
                originalTaskId = taskId,
                assignedCount = count,
                title = task.Title
            });

            return count;
        }
    }
}