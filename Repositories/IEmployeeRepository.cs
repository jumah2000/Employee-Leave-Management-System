using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;

namespace EmployeeLeaveManagementSystem.Repositories;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployees();

    Task<Employee> GetEmployeeById(int id);

    Task<Employee> CreateEmployee(CreateEmployeeDto createEmployeeDto);

    Task<Employee> UpdateEmployee(int id, CreateEmployeeDto dto);

    Task<bool> DeleteEmployee(int id);

    Task<IEnumerable<Leave>> GetEmployeeLeaves(int employeeId);
}