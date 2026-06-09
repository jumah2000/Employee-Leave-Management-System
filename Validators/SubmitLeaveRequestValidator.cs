using EmployeeLeaveManagementSystem.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class SubmitLeaveRequestValidator:  AbstractValidator<SubmitLeaveRequestDto>
{
    public SubmitLeaveRequestValidator()
    {
        // Employee must be selected
        RuleFor(x => x.EmployeeId)
            .GreaterThan(0).WithMessage("Valid EmployeeId is required");

        // Leave type required
        RuleFor(x => x.LeaveType)
            .NotEmpty().WithMessage("Leave type is required");

        // Start date required
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required");

        // End date required
        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required");

        // Business rule: Start date cannot be after end date
        RuleFor(x => x)
            .Must(x => x.StartDate <= x.EndDate)
            .WithMessage("Start date cannot be later than end date");

        // Reason required
        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500);
    }

}