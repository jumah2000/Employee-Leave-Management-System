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

    public async Task<IEnumerable<Leave>> GetAllLeaves()
    {
        var leaves = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .ToListAsync();

        return leaves;
    }

    public async Task<Leave> GetLeaveById(int id)
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

    public async Task<Leave> CreateLeave(CreateLeaveDto dto)
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
        
        //does this empolyee already have a leave that clashes with this new one
        var overlap = await _dbContext.LeaveRequests
            .AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                dto.StartDate <= l.EndDate &&
                dto.EndDate >= l.StartDate);

        if (overlap)
        {
            throw new Exception("Overlapping leave request detected");
        }

        var leave = new Leave
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
    
    
    public async Task<Leave> UpdateLeave(int id, CreateLeaveDto dto)
        {
            var leaveExist = await _dbContext.LeaveRequests
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leaveExist == null)
            {
                throw new Exception("Leave request not found");
            }

            if (dto.StartDate > dto.EndDate)
            {
                throw new Exception("Start Date cannot be later than End Date");
            }

            var allowedStatuses = new[]
            {
                "Pending",
                "Approved",
                "Rejected"
            };

            // since DTO has no Status, keep current or default to Pending
            var status = leaveExist.Status;

            if (!allowedStatuses.Contains(status))
            {
                throw new Exception("Invalid leave status");
            }

            var overlap = await _dbContext.LeaveRequests
                .AnyAsync(l =>
                    l.EmployeeId == leaveExist.EmployeeId &&
                    l.Id != id &&
                    dto.StartDate <= l.EndDate &&
                    dto.EndDate >= l.StartDate);

            if (overlap)
            {
                throw new Exception("Overlapping leave request detected");
            }

            // update fields from DTO
            leaveExist.LeaveType = dto.LeaveType;
            leaveExist.StartDate = dto.StartDate;
            leaveExist.EndDate = dto.EndDate;
            leaveExist.Reason = dto.Reason;

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

    public async Task<IEnumerable<Leave>> GetLeavesByStatus(string status)
    {
        var leaves = await _dbContext.LeaveRequests
            .Include(l => l.Employee)
            .Where(l => l.Status.ToLower() == status.ToLower())
            .ToListAsync();

        return leaves;
    }

    public async Task<Leave> ApproveLeave(int id)
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

    public async Task<Leave> RejectLeave(int id)
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

    public async Task<IEnumerable<Leave>> GetCurrentLeaves()
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
    
    
    
