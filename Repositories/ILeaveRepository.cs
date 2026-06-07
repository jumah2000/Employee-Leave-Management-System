using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    Task<IEnumerable<Leave>> GetAllLeaves();

    Task<Leave> GetLeaveById(int id);

    Task<Leave> CreateLeave(CreateLeaveDto dto);

    Task<Leave> UpdateLeave(int id, CreateLeaveDto dto);

    Task<bool> DeleteLeave(int id);

    Task<IEnumerable<Leave>> GetLeavesByStatus(string status);

    Task<Leave> ApproveLeave(int id);

    Task<Leave> RejectLeave(int id);

    Task<IEnumerable<Leave>> GetCurrentLeaves();
    
    Task<IEnumerable<object>> GetDepartmentStatistics();
}
