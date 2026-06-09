using EmployeeLeaveManagementSystem.DTOs;
using FluentValidation;

namespace EmployeeLeaveManagementSystem.Validators;

public class CreateEmployeeRequestValidator : AbstractValidator<CreateEmployeeRequestDto>
{
    public CreateEmployeeRequestValidator()
    {
        // Full name must not be empty
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Full name is required")
            .MaximumLength(100);

        // Email must be valid format
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");

        // Department must be provided
        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required");
    }

}