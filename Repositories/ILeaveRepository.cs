using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.DTOs.Responses;
using EmployeeLeaveManagementSystem.Enums;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface ILeaveRepository
{
    // BASIC CRUD OPERATIONS
    Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeaves();

    Task<LeaveRequestResponseDto> GetLeaveById(int id);

    Task<LeaveRequestResponseDto> CreateLeave(SubmitLeaveRequestDto dto);

    Task<LeaveRequestResponseDto> UpdateLeave(int id, SubmitLeaveRequestDto dto);

    Task<bool> DeleteLeave(int id);
    
    // APPROVAL WORKFLOW
    Task<LeaveRequestResponseDto> ApproveLeave(int leaveId, LeaveActionRequestDto dto);

    Task<LeaveRequestResponseDto> RejectLeave(int leaveId, LeaveActionRequestDto dto);
    
    // BUSINESS RULE QUERIES
    
    Task<IEnumerable<LeaveRequestResponseDto>> GetLeavesByStatus(LeaveStatus status);

    Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaveHistory(int employeeId);

    Task<IEnumerable<EmployeeResponseDto>> GetEmployeesOnLeave();

    Task<object> GetLeaveStatisticsByDepartment();
}