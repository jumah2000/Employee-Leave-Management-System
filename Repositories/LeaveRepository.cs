using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class LeaveRepository: ILeaveRepository
{
    private readonly ApplicationDbContext _dbContext;

    public LeaveRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<LeaveRequest>> GetAllLeaves()
    {
        var leaves = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .ToListAsync();

        return leaves;
    }

    public async Task<LeaveRequest> GetLeaveById(int id)
    {
        var leave = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            throw new Exception("Leave request not found");
        }

        return leave;
    }

    public async Task<LeaveRequest> CreateLeave(CreateLeaveDto dto)
    {
        var employeeExists = await _dbContext.Employees
            .AnyAsync(e => e.Id == dto.EmployeeId);

        if (!employeeExists)
        {
            throw new Exception("Employee does not exist");
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date");
        }

        var overlap = await _dbContext.LeaveRequests
            .AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                dto.StartDate <= l.EndDate &&
                dto.EndDate >= l.StartDate);

        if (overlap)
        {
            throw new Exception("Overlapping leave request detected");
        }

        var leave = new LeaveRequest
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = "Pending",
            DateCreated = DateTime.UtcNow
        };

        await _dbContext.LeaveRequests.AddAsync(leave);

        await _dbContext.SaveChangesAsync();

        return leave;
    }

    public async Task<LeaveRequest> UpdateLeave(LeaveRequest leaveRequest)
    {
        var leaveExist = await _dbContext.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == leaveRequest.Id);

        if (leaveExist == null)
        {
            throw new Exception("Leave request not found");
        }

        if (leaveRequest.StartDate > leaveRequest.EndDate)
        {
            throw new Exception("Start Date cannot be later than End Date");
        }

        var allowedStatuses = new[]
        {
            "Pending",
            "Approved",
            "Rejected"
        };

        if (!allowedStatuses.Contains(leaveRequest.Status))
        {
            throw new Exception("Invalid leave status");
        }

        var overlap = await _dbContext.LeaveRequests
            .AnyAsync(l =>
                l.EmployeeId == leaveRequest.EmployeeId &&
                l.Id != leaveRequest.Id &&
                leaveRequest.StartDate <= l.EndDate &&
                leaveRequest.EndDate >= l.StartDate);

        if (overlap)
        {
            throw new Exception("Overlapping leave request detected");
        }

        leaveExist.EmployeeId = leaveRequest.EmployeeId;
        leaveExist.LeaveType = leaveRequest.LeaveType;
        leaveExist.StartDate = leaveRequest.StartDate;
        leaveExist.EndDate = leaveRequest.EndDate;
        leaveExist.Reason = leaveRequest.Reason;
        leaveExist.Status = leaveRequest.Status;

        _dbContext.LeaveRequests.Update(leaveExist);

        await _dbContext.SaveChangesAsync();

        return leaveExist;
    }

    public async Task<bool> DeleteLeave(int id)
    {
        var leave = await _dbContext.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            throw new Exception("Leave request not found");
        }

        _dbContext.LeaveRequests.Remove(leave);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<LeaveRequest>> GetLeavesByStatus(string status)
    {
        var leaves = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .Where(l => l.Status.ToLower() == status.ToLower())
            .ToListAsync();

        return leaves;
    }

    public async Task<LeaveRequest> ApproveLeave(int id)
    {
        var leave = await _dbContext.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            throw new Exception("Leave request not found");
        }

        leave.Status = "Approved";

        await _dbContext.SaveChangesAsync();

        return leave;
    }

    public async Task<LeaveRequest> RejectLeave(int id)
    {
        var leave = await _dbContext.LeaveRequests
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
        {
            throw new Exception("Leave request not found");
        }

        leave.Status = "Rejected";

        await _dbContext.SaveChangesAsync();

        return leave;
    }

    public async Task<IEnumerable<LeaveRequest>> GetCurrentLeaves()
    {
        var today = DateTime.Today;

        var leaves = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .Where(l =>
                l.Status == "Approved" &&
                today >= l.StartDate.Date &&
                today <= l.EndDate.Date)
            .ToListAsync();

        return leaves;
    }
    public async Task<IEnumerable<object>> GetDepartmentStatistics()
    {
        var statistics = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .GroupBy(l => l.Employee.Department)
            .Select(g => new
            {
                Department = g.Key,
                TotalLeaves = g.Count()
            })
            .ToListAsync();

        return statistics;
    }
}
    
    
    
