using EmployeeLeaveManagementSystem.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class UpdateEmployeeRequestValidator: AbstractValidator<UpdateEmployeeRequestDto>
{
    public UpdateEmployeeRequestValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required");
    }

}