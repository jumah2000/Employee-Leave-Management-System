using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.DTOs.Responses;
using EmployeeLeaveManagementSystem.Enums;
using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly ApplicationDbContext _context;

    // I inject DbContext here so we can access the database
    public EmployeeRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET ALL EMPLOYEES
    public async Task<IEnumerable<EmployeeResponseDto>> GetAllEmployees()
    {
        // I fetch all employees from database
        // Then I convert them into Response DTO so we don't expose raw database models
        return await _context.Employees
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FullName = e.FullName,
                Email = e.Email,
                Department = e.Department,
                DateJoined = e.DateJoined
            })
            .ToListAsync();
    }
    
    // GET EMPLOYEE BY ID
    public async Task<EmployeeResponseDto> GetEmployeeById(int id)
    {
        // search for a single employee using their ID
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        // If employee does not exist, its stop execution
        if (employee == null)
            throw new Exception("Employee not found");

        // this map database model to DTO before returning
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };
    }
    
    // CREATE EMPLOYEE
    // =========================
    public async Task<EmployeeResponseDto> CreateEmployee(CreateEmployeeRequestDto dto)
    {
        // This first check if an employee already exists with same email
        // This prevents duplicate employee records
        var exists = await _context.Employees
            .AnyAsync(e => e.Email == dto.Email);

        if (exists)
            throw new Exception("Employee with this email already exists");

        // We create a new employee object from request data
        var employee = new Employee
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Department = dto.Department,
            DateJoined = DateTime.UtcNow
        };

        // Add employee to database
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        // Return clean response DTO
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };
    }
    
    // UPDATE EMPLOYEE
    public async Task<EmployeeResponseDto> UpdateEmployee(int id, UpdateEmployeeRequestDto dto)
    {
        // Find employee by ID
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        // If not found, stop execution
        if (employee == null)
            throw new Exception("Employee not found");

        // Update only allowed fields
        employee.FullName = dto.FullName;
        employee.Email = dto.Email;
        employee.Department = dto.Department;

        // Save changes to database
        await _context.SaveChangesAsync();

        // Return updated data as DTO
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Department = employee.Department,
            DateJoined = employee.DateJoined
        };
    }
    
    // DELETE EMPLOYEE
    
    public async Task<bool> DeleteEmployee(int id)
    {
        // Find employee first
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id);

        if (employee == null)
            throw new Exception("Employee not found");

        // BUSINESS RULE:
        // We cannot delete an employee who currently has active leave
        // (Pending or Processing means leave is still ongoing workflow)
        var hasActiveLeave = await _context.Leaves
            .AnyAsync(l =>
                l.EmployeeId == id &&
                (l.Status == LeaveStatus.Pending ||
                 l.Status == LeaveStatus.Processing));

        if (hasActiveLeave)
            throw new Exception("Cannot delete employee with active leave requests");

        // Remove employee from database
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return true;
    }
    
    // GET EMPLOYEE LEAVE HISTORY
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaves(int employeeId)
    {
        // First confirm employee exists
        var employeeExists = await _context.Employees
            .AnyAsync(e => e.Id == employeeId);

        if (!employeeExists)
            throw new Exception("Employee not found");

        // Get all leaves for that employee
        // Include approvals so we can show full history
        return await _context.Leaves
            .Where(l => l.EmployeeId == employeeId)
            .Include(l => l.Approvals)
            .Select(l => new LeaveRequestResponseDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                LeaveType = l.LeaveType,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,

                // Convert enum to string for frontend readability
                Status = l.Status.ToString(),

                DateCreated = l.DateCreated,

                // Map each approval record into a clean DTO
                Approvals = l.Approvals.Select(a => new LeaveApprovalResponseDto
                {
                    ApproverId = a.ApproverId,
                    Action = a.Action.ToString(),
                    Reason = a.Reason,
                    DateActed = a.DateActed
                }).ToList()
            })
            .ToListAsync();
    }
    
    // GET EMPLOYEES CURRENTLY ON LEAVE
    public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesOnLeave()
    {
        var today = DateTime.UtcNow;

        // We check only APPROVED leaves that are currently active (date range match)
        return await _context.Leaves
            .Include(l => l.Employee)
            .Where(l =>
                l.Status == LeaveStatus.Approved &&
                l.StartDate <= today &&
                l.EndDate >= today)
            .Select(l => new EmployeeResponseDto
            {
                Id = l.Employee.Id,
                FullName = l.Employee.FullName,
                Email = l.Employee.Email,
                Department = l.Employee.Department,
                DateJoined = l.Employee.DateJoined
            })

            // Remove duplicates in case employee has multiple leave records
            .Distinct()
            .ToListAsync();
    }

}

