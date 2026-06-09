using EmployeeLeaveManagementSystem.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class LeaveActionRequestValidator: AbstractValidator<LeaveActionRequestDto>
{
    public LeaveActionRequestValidator()
    {
        // Approver must be valid
        RuleFor(x => x.ApproverId)
            .GreaterThan(0).WithMessage("ApproverId is required");

        // Reason optional by default (we will override for reject in controller/repo logic)
        RuleFor(x => x.Reason)
            .MaximumLength(500);

        // NOTE:
        // Business rule enforcement:
        // - Reject requires reason
        // - Approve can be optional
        // This is enforced in repository/controller since FluentValidation
        // cannot detect endpoint type directly
    }

}