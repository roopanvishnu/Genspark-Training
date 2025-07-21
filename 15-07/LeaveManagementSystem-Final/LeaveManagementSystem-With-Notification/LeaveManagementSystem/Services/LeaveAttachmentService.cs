using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using LeaveManagementSystem.Repositories;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Services
{
    public class LeaveAttachmentService : ILeaveAttachmentService
    {
        private readonly IRepository<Guid, LeaveAttachment> _repository;
        private readonly ILeaveRequestService _leaveRequestService;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;

        public LeaveAttachmentService(
            IRepository<Guid, LeaveAttachment> repository,
            ILeaveRequestService leaveRequestService,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _leaveRequestService = leaveRequestService;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
        }

        public async Task<LeaveAttachment> CreateAsync(LeaveAttachmentDto dto)
        {
            try
            {
                byte[] fileBytes;
                using (var ms = new MemoryStream())
                {
                    await dto.File.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                var attachment = new LeaveAttachment
                {
                    Id = Guid.NewGuid(),
                    LeaveRequestId = dto.LeaveRequestId,
                    FileName = dto.File.FileName,
                    FileContent = fileBytes,
                    UploadedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                var result = await _repository.Add(attachment);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "INSERTED",
                    "LEAVEATTACHMENT",
                    result.Id
                );

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while creating the leave attachment.", ex);
            }
        }

        public async Task<LeaveAttachment> GetByIdAsync(Guid id)
        {
            try
            {
                var attachment = await _repository.Get(id);
                if (attachment == null)
                    throw new KeyNotFoundException($"LeaveAttachment with Id {id} not found.");
                return attachment;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving the leave attachment.", ex);
            }
        }

        public async Task<IEnumerable<LeaveAttachment>> GetByLeaveRequestIdAsync(Guid leaveRequestId)
        {
            try
            {
                var leaveRequest = await _leaveRequestService.GetByIdAsync(leaveRequestId);
                if (leaveRequest == null)
                    throw new KeyNotFoundException($"LeaveRequest with Id {leaveRequestId} not found.");

                var allAttachments = await _repository.GetAll();

                var filtered = allAttachments
                    .Where(a => a.LeaveRequestId == leaveRequestId && !a.IsDeleted);

                return filtered;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while retrieving leave attachments by LeaveRequestId.", ex);
            }
        }


        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var deleted = await _repository.Delete(id);

                if (deleted != null)
                {
                    await _auditLogService.LogAsync(
                        _currentUserService.GetUserId(),
                        "DELETED",
                        "LEAVEATTACHMENT",
                        id
                    );
                }

                return deleted != null;
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred while deleting the leave attachment.", ex);
            }
        }
    }
}
