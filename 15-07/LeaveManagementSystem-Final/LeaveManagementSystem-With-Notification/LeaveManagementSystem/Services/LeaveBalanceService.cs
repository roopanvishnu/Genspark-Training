using LeaveManagementSystem.Interfaces;
using LeaveManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagementSystem.Models.DTOs;
using AutoMapper;

namespace LeaveManagementSystem.Services
{
    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly IRepository<Guid, LeaveBalance> _leaveBalanceRepository;
        private readonly IRepository<Guid, LeaveType> _leaveTypeRepository;
        private readonly IRepository<Guid, User> _userRepository;
        private readonly IMapper _mapper;

        public LeaveBalanceService(IRepository<Guid, LeaveBalance> leaveBalanceRepository, IRepository<Guid, LeaveType> leaveTypeRepository, IRepository<Guid, User> userRepository, IMapper mapper)
        {
            _leaveBalanceRepository = leaveBalanceRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }


        public async Task<UserLeaveBalanceResponseDto> GetLeaveBalancesForUserAsync(Guid userId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var leaveBalances = user.LeaveBalances;

            var leaveBalanceDtos = _mapper.Map<List<LeaveBalanceResponseDto>>(leaveBalances);

            return new UserLeaveBalanceResponseDto
            {
                UserId = user.Id,
                UserName = user.Username,
                LeaveBalances = leaveBalanceDtos
            };
        }

        public async Task<UserLeaveBalanceForTypeResponseDto> GetLeaveBalanceForTypeAsync(Guid userId, Guid leaveTypeId)
        {
            var user = await _userRepository.Get(userId);
            if (user == null)
                throw new KeyNotFoundException($"User with ID {userId} not found");

            var leaveBalance = user.LeaveBalances.FirstOrDefault(lb => lb.LeaveTypeId == leaveTypeId);
            if (leaveBalance == null)
                throw new KeyNotFoundException($"Leave Balance for LeaveType {leaveTypeId} not found for User {userId}");

            return new UserLeaveBalanceForTypeResponseDto
            {
                UserId = user.Id,
                UserName = user.Username,
                LeaveBalance = _mapper.Map<LeaveBalanceResponseDto>(leaveBalance)
            };
        }

        public async Task InitializeLeaveBalancesForUserAsync(Guid userId)
        {
            var existingBalances = (await _leaveBalanceRepository.GetAll())
                                    .Where(lb => lb.UserId == userId)
                                    .ToList();

            if (existingBalances.Any())
                return; 

            var leaveTypes = await _leaveTypeRepository.GetAll();
            var leaveBalances = leaveTypes.Select(lt => new LeaveBalance
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LeaveTypeId = lt.Id,
                TotalLeaves = lt.StandardLeaveCount, 
                UsedLeaves = 0
            }).ToList();

            foreach (var balance in leaveBalances)
            {
                await _leaveBalanceRepository.Add(balance);
            }
        }

        public async Task InitializeLeaveBalanceForNewLeaveTypeAsync(Guid userId, Guid leaveTypeId, int standardLeaveCount)
        {
            var allBalances = await _leaveBalanceRepository.GetAll();
            var existing = allBalances.FirstOrDefault(lb => lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

            if (existing != null)
                return; 

            var newBalance = new LeaveBalance
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                LeaveTypeId = leaveTypeId,
                TotalLeaves = standardLeaveCount,
                UsedLeaves = 0
            };

            await _leaveBalanceRepository.Add(newBalance);
        }

        public async Task DeductLeaveBalanceAsync(Guid userId, Guid leaveTypeId, int days)
        {
            var allBalances = await _leaveBalanceRepository.GetAll();
            var balance = allBalances.FirstOrDefault(lb => lb.UserId == userId && lb.LeaveTypeId == leaveTypeId);

            if (balance == null)
                throw new Exception("Leave balance not found.");

            balance.UsedLeaves += days;

            await _leaveBalanceRepository.Update(balance.Id, balance);
        }

        public async Task ResetLeaveBalancesForUserAsync(Guid userId)
        {
            var allBalances = await _leaveBalanceRepository.GetAll();
            var userBalances = allBalances.Where(lb => lb.UserId == userId).ToList();

            foreach (var balance in userBalances)
            {
                balance.UsedLeaves = 0; 
                await _leaveBalanceRepository.Update(balance.Id, balance);
            }
        }
    }
}