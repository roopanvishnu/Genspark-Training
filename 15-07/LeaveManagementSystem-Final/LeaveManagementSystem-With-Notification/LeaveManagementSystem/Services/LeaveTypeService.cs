using AutoMapper;
using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using LeaveManagementSystem.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagementSystem.Services
{
    public class LeaveTypeService : ILeaveTypeService
    {
        private readonly IRepository<Guid, LeaveType> _repository;
        private readonly IAuditLogService _auditLogService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public LeaveTypeService(
            IRepository<Guid, LeaveType> repository,
            IAuditLogService auditLogService,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _repository = repository;
            _auditLogService = auditLogService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }


        public async Task<IEnumerable<LeaveTypeResponseDto>> GetAllAsync(int pageNumber, int pageSize, string searchTerm, string sortBy, string sortOrder)
        {
            try
            {
                var leaveTypes = await _repository.GetAll(); 
                var leaveTypeDtos = _mapper.Map<List<LeaveTypeResponseDto>>(leaveTypes);

                // Search 
                
                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    leaveTypeDtos = leaveTypeDtos
                        .Where(lt => (!string.IsNullOrEmpty(lt.Name) && lt.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                                     (!string.IsNullOrEmpty(lt.Description) && lt.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        )
                        .ToList();
                }

                // Sort
                leaveTypeDtos = sortBy?.ToLower() switch
                {
                    "name" => (sortOrder == "desc") ? leaveTypeDtos.OrderByDescending(lt => lt.Name).ToList() : leaveTypeDtos.OrderBy(lt => lt.Name).ToList(),
                    _ => (sortOrder == "desc") ? leaveTypeDtos.OrderByDescending(lt => lt.CreatedAt).ToList() : leaveTypeDtos.OrderBy(lt => lt.CreatedAt).ToList(),
                };

                // Pagination
                var totalCount = leaveTypeDtos.Count;
                var paginatedItems = leaveTypeDtos
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                return paginatedItems;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving leave types.", ex);
            }
        }


        public async Task<LeaveType> GetByIdAsync(Guid id)
        {
            try
            {
                var leaveType = await _repository.Get(id);
                if (leaveType == null)
                    throw new KeyNotFoundException($"LeaveType with id {id} not found.");
                return leaveType;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the leave type.", ex);
            }
        }

        public async Task<LeaveType> CreateAsync(LeaveTypeDto dto)
        {
            try
            {
                var leaveType = _mapper.Map<LeaveType>(dto);
                leaveType.Id = Guid.NewGuid();
                leaveType.CreatedAt = DateTime.UtcNow;
                leaveType.CreatedBy = _currentUserService.GetUserId();

                var createdLeaveType = await _repository.Add(leaveType);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "INSERTED",
                    "LEAVETYPE",
                    createdLeaveType.Id
                );

                return createdLeaveType;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the leave type.", ex);
            }
        }

        public async Task<LeaveType> UpdateAsync(Guid id, LeaveTypeDto dto)
        {
            try
            {
                var existing = await _repository.Get(id);
                if (existing == null)
                    throw new KeyNotFoundException($"LeaveType with id {id} not found.");

                _mapper.Map(dto, existing);
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = _currentUserService.GetUserId();

                var updatedLeaveType = await _repository.Update(id, existing);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "UPDATED",
                    "LEAVETYPE",
                    updatedLeaveType.Id
                );

                return updatedLeaveType;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex) 
            {
                throw new Exception($"An error occurred while updating the leave type. {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                var existing = await _repository.Get(id);
                if (existing == null)
                    throw new KeyNotFoundException($"LeaveType with id {id} not found.");

                await _repository.Delete(id);

                await _auditLogService.LogAsync(
                    _currentUserService.GetUserId(),
                    "DELETED",
                    "LEAVETYPE",
                    id
                );

                return true;
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the leave type.", ex);
            }
        }
    }
}
