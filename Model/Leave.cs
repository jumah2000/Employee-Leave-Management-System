using EmployeeLeaveManagementSystem.Enums;

namespace EmployeeLeaveManagementSystem.Model;

public class Leave
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; }
    public LeaveStatus Status { get; set; }= LeaveStatus.Pending;
    public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    public Employee Employee { get; set; }

    public ICollection<LeaveApproval> Approvals { get; set; } = new List<LeaveApproval>();


}