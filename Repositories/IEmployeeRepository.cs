using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployees();

    Task<Employee> GetEmployeeById(int id);

    Task<Employee> CreateEmployee(CreateEmployeeDto createEmployeeDto);

    Task<Employee> UpdateEmployee(Employee employee);

    Task<bool> DeleteEmployee(int id);

    Task<IEnumerable<LeaveRequest>> GetEmployeeLeaves(int employeeId);
}