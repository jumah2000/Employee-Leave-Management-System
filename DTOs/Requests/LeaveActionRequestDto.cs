namespace EmployeeLeaveManagementSystem.DTOs;

public class LeaveActionRequestDto
{
        public int ApproverId { get; set; }

        public string? Reason { get; set; }
}