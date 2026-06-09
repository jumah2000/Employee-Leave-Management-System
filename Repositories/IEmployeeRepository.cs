using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.DTOs.Responses;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface IEmployeeRepository
{
    
    Task<IEnumerable<EmployeeResponseDto>> GetAllEmployees();

    Task<EmployeeResponseDto> GetEmployeeById(int id);

    Task<EmployeeResponseDto> CreateEmployee(CreateEmployeeRequestDto dto);

    Task<EmployeeResponseDto> UpdateEmployee(int id, UpdateEmployeeRequestDto dto);

    Task<bool> DeleteEmployee(int id);

    Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaves(int employeeId);
    
    Task<IEnumerable<EmployeeResponseDto>> GetEmployeesOnLeave();

}