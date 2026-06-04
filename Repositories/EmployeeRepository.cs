using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class EmployeeRepository : IEmployeeRepository

{
    private readonly ApplicationDbContext _dbContext;

   public EmployeeRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

      public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        var employees = await _dbContext.Employees.ToListAsync();

        return employees;
    }

    public async Task<Employee> GetEmployeeById(int id)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        return employee;
    }

    public async Task<Employee> CreateEmployee(CreateEmployeeDto createEmployeeDto)
    {
        if (string.IsNullOrWhiteSpace(createEmployeeDto.FullName))
        {
            throw new Exception("Full Name is required");
        }

        if (string.IsNullOrWhiteSpace(createEmployeeDto.Email))
        {
            throw new Exception("Email is required");
        }

        if (string.IsNullOrWhiteSpace(createEmployeeDto.Department))
        {
            throw new Exception("Department is required");
        }

        var employeeExists = await _dbContext.Employees
            .AnyAsync(e => e.Email.ToLower() == createEmployeeDto.Email.ToLower());

        if (employeeExists)
        {
            throw new Exception("Employee with this email already exists");
        }

        var employee = new Employee
        {
            FullName = createEmployeeDto.FullName,
            Email = createEmployeeDto.Email,
            Department = createEmployeeDto.Department,
            DateJoined = createEmployeeDto.DateJoined
        };

        await _dbContext.Employees.AddAsync(employee);

        await _dbContext.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee> UpdateEmployee(Employee employee)
    {
        var employeeExist = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == employee.Id);

        if (employeeExist == null)
        {
            throw new Exception("Employee not found");
        }

        if (string.IsNullOrWhiteSpace(employee.FullName))
        {
            throw new Exception("Full Name is required");
        }

        if (string.IsNullOrWhiteSpace(employee.Email))
        {
            throw new Exception("Email is required");
        }

        if (string.IsNullOrWhiteSpace(employee.Department))
        {
            throw new Exception("Department is required");
        }

        var emailExists = await _dbContext.Employees
            .AnyAsync(e =>
                e.Email.ToLower() == employee.Email.ToLower()
                && e.Id != employee.Id);

        if (emailExists)
        {
            throw new Exception("Another employee already uses this email");
        }

        employeeExist.FullName = employee.FullName;
        employeeExist.Email = employee.Email;
        employeeExist.Department = employee.Department;
        employeeExist.DateJoined = employee.DateJoined;

        _dbContext.Employees.Update(employeeExist);

        await _dbContext.SaveChangesAsync();

        return employeeExist;
    }

    public async Task<bool> DeleteEmployee(int id)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        _dbContext.Employees.Remove(employee);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<LeaveRequest>> GetEmployeeLeaves(int employeeId)
    {
        var employee = await _dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == employeeId);

        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

        var leaves = await _dbContext.LeaveRequests
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.DateCreated)
            .ToListAsync();

        return leaves;
    }
}
