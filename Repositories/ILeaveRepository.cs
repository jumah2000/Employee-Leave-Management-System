using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    Task<IEnumerable<LeaveRequest>> GetAllLeaves();

    Task<LeaveRequest> GetLeaveById(int id);

    Task<LeaveRequest> CreateLeave(CreateLeaveDto dto);

    Task<LeaveRequest> UpdateLeave(LeaveRequest leave);

    Task<bool> DeleteLeave(int id);

    Task<IEnumerable<LeaveRequest>> GetLeavesByStatus(string status);

    Task<LeaveRequest> ApproveLeave(int id);

    Task<LeaveRequest> RejectLeave(int id);

    Task<IEnumerable<LeaveRequest>> GetCurrentLeaves();
    
    Task<IEnumerable<object>> GetDepartmentStatistics();
}
