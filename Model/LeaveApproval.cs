using EmployeeLeaveManagementSystem.Enums;

namespace EmployeeLeaveManagementSystem.Model;

public class LeaveApproval
{
    public int Id { get; set; }

    public int LeaveId { get; set; }

    public Leave Leave { get; set; }

    public int ApproverId { get; set; }

    public ApprovalAction Action { get; set; }

    public string? Reason { get; set; }

    public DateTime DateActed { get; set; } = DateTime.UtcNow;
}
