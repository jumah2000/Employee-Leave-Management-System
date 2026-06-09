using EmployeeLeaveManagementSystem.Data;
using EmployeeLeaveManagementSystem.DTOs;
using EmployeeLeaveManagementSystem.DTOs.Responses;
using EmployeeLeaveManagementSystem.Enums;
using EmployeeLeaveManagementSystem.Model;
using Microsoft.EntityFrameworkCore;

namespace EmployeeLeaveManagementSystem.Repositories;

public class LeaveRepository: ILeaveRepository
{
    private readonly ApplicationDbContext _context;

    // I inject DbContext so we can access Employees, Leaves, and Approvals tables
    public LeaveRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    // GET ALL LEAVES
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetAllLeaves()
    {
        // Fetch all leave requests including their approval history
        return await _context.Leaves
            .Include(l => l.Approvals)
            .Select(l => new LeaveRequestResponseDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                LeaveType = l.LeaveType,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                DateCreated = l.DateCreated,

                // Map approval history into clean DTOs
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
    
    // GET LEAVE BY ID
    public async Task<LeaveRequestResponseDto> GetLeaveById(int id)
    {
        var leave = await _context.Leaves
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (leave == null)
            throw new Exception("Leave request not found");

        return new LeaveRequestResponseDto
        {
            Id = leave.Id,
            EmployeeId = leave.EmployeeId,
            LeaveType = leave.LeaveType,
            StartDate = leave.StartDate,
            EndDate = leave.EndDate,
            Reason = leave.Reason,
            Status = leave.Status.ToString(),
            DateCreated = leave.DateCreated,

            Approvals = leave.Approvals.Select(a => new LeaveApprovalResponseDto
            {
                ApproverId = a.ApproverId,
                Action = a.Action.ToString(),
                Reason = a.Reason,
                DateActed = a.DateActed
            }).ToList()
        };
    }
    
    // CREATE / SUBMIT LEAVE
    public async Task<LeaveRequestResponseDto> CreateLeave(SubmitLeaveRequestDto dto)
    {
        // Check if employee exists
        var employee = await _context.Employees.FindAsync(dto.EmployeeId);

        if (employee == null)
            throw new Exception("Employee not found");

        // Business Rule: Start date must not be after end date
        if (dto.StartDate > dto.EndDate)
            throw new Exception("Start date cannot be later than end date");

        // Business Rule: Prevent overlapping leave requests
        var overlap = await _context.Leaves.AnyAsync(l =>
            l.EmployeeId == dto.EmployeeId &&
            dto.StartDate <= l.EndDate &&
            dto.EndDate >= l.StartDate);

        if (overlap)
            throw new Exception("Employee already has an overlapping leave request");

        //  Create leave request (default status = Pending)
        var leave = new Leave
        {
            EmployeeId = dto.EmployeeId,
            LeaveType = dto.LeaveType,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending
        };

        _context.Leaves.Add(leave);
        await _context.SaveChangesAsync();

        return await GetLeaveById(leave.Id);
    }
    
    //UpdateLeave
    public async Task<LeaveRequestResponseDto> UpdateLeave(int id, SubmitLeaveRequestDto dto)
    { 
        // Find existing leave request
            var leave = await _context.Leaves
                .Include(l => l.Approvals)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leave == null)
                throw new Exception("Leave not found");

            // BUSINESS RULE: You cannot update a leave that is already approved or rejected
            // Because workflow has already been completed
            if (leave.Status == LeaveStatus.Approved ||
                leave.Status == LeaveStatus.Rejected)
            {
                throw new Exception("Cannot update a completed leave request");
            }

            // BUSINESS RULE: Start date must not be after end date
            if (dto.StartDate > dto.EndDate)
                throw new Exception("Start date cannot be greater than end date");

            // BUSINESS RULE: Prevent overlapping leave (excluding itself)
            var overlap = await _context.Leaves.AnyAsync(l =>
                l.EmployeeId == dto.EmployeeId &&
                l.Id != id &&
                dto.StartDate <= l.EndDate &&
                dto.EndDate >= l.StartDate);

            if (overlap)
                throw new Exception("Updated leave overlaps with another leave request");

            // Update fields
            leave.LeaveType = dto.LeaveType;
            leave.StartDate = dto.StartDate;
            leave.EndDate = dto.EndDate;
            leave.Reason = dto.Reason;

            // NOTE:
            // We do NOT reset status or approvals because workflow integrity must be preserved

            await _context.SaveChangesAsync();

            return await GetLeaveById(id);
    }
    
    // APPROVE LEAVE (2-STEP WORKFLOW)
    public async Task<LeaveRequestResponseDto> ApproveLeave(int leaveId, LeaveActionRequestDto dto)
    {
        var leave = await _context.Leaves
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == leaveId);

        if (leave == null)
            throw new Exception("Leave not found");

        // BUSINESS RULE: Employee cannot approve their own leave
        if (leave.EmployeeId == dto.ApproverId)
            throw new Exception("Self-approval is not allowed");

        // BUSINESS RULE: Each employee can act only once
        var alreadyActed = leave.Approvals.Any(a => a.ApproverId == dto.ApproverId);

        if (alreadyActed)
            throw new Exception("You have already acted on this leave request");

        // Record approval action
        leave.Approvals.Add(new LeaveApproval
        {
            ApproverId = dto.ApproverId,
            Action = ApprovalAction.Approve,
            Reason = dto.Reason
        });
        
        // STATE TRANSITION LOGIC
        // FIRST APPROVAL → Pending → Processing
        if (leave.Approvals.Count == 1)
        {
            leave.Status = LeaveStatus.Processing;
        }

        // SECOND APPROVAL → Processing → Approved
        else if (leave.Approvals.Count == 2)
        {
            leave.Status = LeaveStatus.Approved;
        }

        await _context.SaveChangesAsync();

        return await GetLeaveById(leave.Id);
    }
    
    // REJECT LEAVE (IMMEDIATE TERMINATION)
    public async Task<LeaveRequestResponseDto> RejectLeave(int leaveId, LeaveActionRequestDto dto)
    {
        var leave = await _context.Leaves
            .Include(l => l.Approvals)
            .FirstOrDefaultAsync(l => l.Id == leaveId);

        if (leave == null)
            throw new Exception("Leave not found");

        // BUSINESS RULE: No self rejection
        if (leave.EmployeeId == dto.ApproverId)
            throw new Exception("Self-rejection is not allowed");

        // BUSINESS RULE: One action per employee only
        var alreadyActed = leave.Approvals.Any(a => a.ApproverId == dto.ApproverId);

        if (alreadyActed)
            throw new Exception("You have already acted on this leave request");

        // Record rejection
        leave.Approvals.Add(new LeaveApproval
        {
            ApproverId = dto.ApproverId,
            Action = ApprovalAction.Reject,
            Reason = dto.Reason
        });
        
        // STATE RULE: ANY REJECTION = FINAL STATE
        leave.Status = LeaveStatus.Rejected;

        await _context.SaveChangesAsync();

        return await GetLeaveById(leave.Id);
    }
    
    // DELETE LEAVE
    public async Task<bool> DeleteLeave(int id)
    {
        var leave = await _context.Leaves.FindAsync(id);

        if (leave == null)
            throw new Exception("Leave not found");

        // Business rule: prevent deleting processed workflow leaves
        if (leave.Status == LeaveStatus.Processing)
            throw new Exception("Cannot delete a leave that is already under processing");

        _context.Leaves.Remove(leave);
        await _context.SaveChangesAsync();

        return true;
    }
    
    // GET LEAVES BY STATUS
    public async Task<IEnumerable<LeaveRequestResponseDto>> GetLeavesByStatus(LeaveStatus status)
    {
        return await _context.Leaves
            .Where(l => l.Status == status)
            .Include(l => l.Approvals)
            .Select(l => new LeaveRequestResponseDto
            {
                Id = l.Id,
                EmployeeId = l.EmployeeId,
                LeaveType = l.LeaveType,
                StartDate = l.StartDate,
                EndDate = l.EndDate,
                Reason = l.Reason,
                Status = l.Status.ToString(),
                DateCreated = l.DateCreated
            })
            .ToListAsync();
    }
    
    //Leave History

    public async Task<IEnumerable<LeaveRequestResponseDto>> GetEmployeeLeaveHistory(int employeeId)
    { 
        // First, confirm the employee actually exists
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.Id == employeeId);

            if (!employeeExists)
                throw new Exception("Employee not found");

            // Fetch all leave records for this employee
            // Include approvals so we get full audit trail
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

                    // Convert enum to string for client readability
                    Status = l.Status.ToString(),

                    DateCreated = l.DateCreated,

                    // Map approval history (who approved/rejected and when)
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

    
    // EMPLOYEES ON LEAVE (GLOBAL VIEW)
    public async Task<IEnumerable<EmployeeResponseDto>> GetEmployeesOnLeave()
    {
        var today = DateTime.UtcNow;

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
            .Distinct()
            .ToListAsync();
    }
    
    // LEAVE STATISTICS (BASIC VERSION)
    public async Task<object> GetLeaveStatisticsByDepartment()
    {
        return await _context.Leaves
            .Include(l => l.Employee)
            .GroupBy(l => l.Employee.Department)
            .Select(g => new
            {
                Department = g.Key,
                TotalLeaves = g.Count(),
                Approved = g.Count(x => x.Status == LeaveStatus.Approved),
                Pending = g.Count(x => x.Status == LeaveStatus.Pending),
                Processing = g.Count(x => x.Status == LeaveStatus.Processing),
                Rejected = g.Count(x => x.Status == LeaveStatus.Rejected)
            })
            .ToListAsync();
    }

}
    
    
    
