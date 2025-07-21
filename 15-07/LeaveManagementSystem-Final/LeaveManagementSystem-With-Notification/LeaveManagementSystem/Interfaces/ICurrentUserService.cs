using LeaveManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaveManagementSystem.Models.DTOs;

namespace LeaveManagementSystem.Interfaces
{

    public interface ICurrentUserService
    {
        Guid GetUserId();
        string? GetEmail();
        string? GetRole();
    }

}