using System.Text.Json.Serialization;

namespace EmployeeLeaveManagementSystem.Model;

public class Employee
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string Department { get; set; }
    public DateTime DateJoined { get; set; }
    
    [JsonIgnore]
    public ICollection<Leave> LeaveRequests { get; set; }
}